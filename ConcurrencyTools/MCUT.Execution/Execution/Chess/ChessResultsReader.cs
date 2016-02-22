/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

﻿using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.Concurrency.TestTools.UnitTesting.Chess;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;

namespace Microsoft.Concurrency.TestTools.Execution.Chess
{
    public class ChessResultsReader
    {

        private string resultfile;
        private string schedulefile;
        private string tracefile;

        public ChessResultsReader(string outputPathPrefix)
        {
            resultfile = outputPathPrefix + "results.xml";
            schedulefile = outputPathPrefix + "sched";
            tracefile = outputPathPrefix + "trace";
        }

        // access results.xml
        public XElement GetChessTestResults()
        {
            FileInfo resultfile = new FileInfo(this.resultfile);
            if (!resultfile.Exists)
            {
                if (!resultfile.Directory.Exists)
                    return null;

                // JAM (1/16/2011): This is supporting of old code. I think it was a Yeti thing but is nolonger used.
                //// try to find it in testresults folder
                //DirectoryInfo[] subdirs = resultfile.Directory.GetDirectories();
                //for (int i = 0; i < subdirs.Length; i++)
                //    if (subdirs[i].Name == "TestResults")
                //    {
                //        DirectoryInfo[] subsubdirs = subdirs[i].GetDirectories();
                //        if (subdirs.Length > 0)
                //            resultfile = new FileInfo(subsubdirs[0].FullName + @"\Out\results.xml");
                //    }
                //if (!resultfile.Exists)
                //    return null;
            }

            XElement xresults = ReadResultFile();

            // Remove the final stats so it will always be at the end
            XElement xfinalStats = xresults.Element(XChessNames.FinalStats);
            if (xfinalStats != null)
                xfinalStats.Remove();
            XElement xendtime = xresults.Element(XChessNames.EndTime);
            if (xendtime != null)
                xendtime.Remove();

            // action: repro last schedule
            XElement sched = ChessScheduleUtil.CreateXScheduleFromFile(schedulefile);
            if (sched != null)
            {
                xresults.Add(new XElement(XChessNames.Action, new XAttribute(XNames.AName, XChessNames.Actions.ReproLastSchedule)));
                xresults.Add(sched);
            }

            // action: repeat
            XElement repeataction = new XElement(XChessNames.Action, new XAttribute(XNames.AName, XChessNames.Actions.Repeat));
            //XElement inputsched = ReadScheduleFromFile(inputschedulefile);
            //if (inputsched != null)
            //{
            //    repeataction.Add(inputsched);
            //}
            if (xresults.Element(XNames.Commandline) != null && xresults.Element(XNames.Commandline).Element(XChessNames.Schedule) != null)
            {
                // Create a copy of the schedule that was an input
                repeataction.Add(new XElement(xresults.Element(XNames.Commandline).Element(XChessNames.Schedule)));
            }
            xresults.Add(repeataction);

            // action: view 
            if (new FileInfo(tracefile).Exists)
                xresults.Add(new XElement(XChessNames.Action, new XAttribute(XNames.AName, XChessNames.Actions.View)));

            // Add back the finalStats and endtime at the end
            xresults.Add(xfinalStats);
            xresults.Add(xendtime);

            return xresults;
        }

        /// <summary>Reads the raw results.xml file.</summary>
        /// <returns></returns>
        private XElement ReadResultFile()
        {
            // first attempt: load file directly
            try
            {
                XElement xresults = XDocument.Load(resultfile).Root;
                if (xresults != null)
                {
                    xresults.Remove(); // take out of doc
                    return xresults;
                }
            }
            catch (Exception) { }

            // second attempt: try "fixing" results file by adding </results>"
            try
            {
                string contents = new StreamReader(resultfile).ReadToEnd();
                contents = contents + "</" + XChessNames.Results.LocalName + ">";
                XElement xresults = XDocument.Load(new StringReader(contents)).Root;
                if (xresults != null)
                {
                    xresults.Remove(); // take out of this document.
                    var xerrResult = GetErrorResult(false);

                    // Lets add the last successful schedule
                    // action: repro & continue
                    XElement sched = ChessScheduleUtil.CreateXScheduleFromFile(schedulefile);
                    if (sched != null)
                    {
                        xerrResult.Add(new XElement(XChessNames.Action, new XAttribute(XNames.AName, XChessNames.Actions.Repro)));
                        //xerrResult.Add(new XElement(XChessNames.Action, new XAttribute(XNames.AName, XChessNames.Actions.Continue)));
                        xerrResult.Add(sched);
                    }

                    xresults.Add(xerrResult);
                    return xresults;
                }
            }
            catch (Exception) { }
            {
                XElement xresults = new XElement(XChessNames.Results, GetErrorResult(true));
                return xresults;
            }
        }

        private XElement GetErrorResult(bool truncated)
        {
            return new XElement(XChessNames.Result
                , new XElement(XChessNames.Label, MChessResultType.Error.ToLabel())
                , new XElement(XChessNames.Description, "Abnormal termination" + (truncated ? " (results corrupted)" : ""))
                );
        }

    }
}
