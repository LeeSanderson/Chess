/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Microsoft.ConcurrencyExplorer
{
    internal class Format
    {
        internal Color threadColor;
        internal Color lineColor;
        internal Color textColor;
        internal bool fatBorder;
        internal Brush[] fillBrushes;
        internal string text;
        internal string objs;
        internal Font font;
        internal bool cross;
    }

    internal class Formatter : IDisposable
    {

        internal bool useOldStyle;

        internal Formatter(bool oldStyle, IIndexConverter converter)
        {
            useOldStyle = oldStyle;
            line = (oldStyle ? Color.Black : Color.White);
            this.converter = converter;
        }

        private Font textfont = new Font("Times New Roman", 8);
       
        private Color line;

        public void Dispose()
        {
            textfont.Dispose();
            GC.SuppressFinalize(this);
        }

        private static string AssembleObjList(EventRecord rec)
        {
            string objlist = "";
            IEnumerator<KeyValuePair<int, int>> en = rec.vars.GetEnumerator();
            while (en.MoveNext())
            {
                if (!String.IsNullOrEmpty(objlist))
                    objlist = objlist + ",";
                objlist = objlist + en.Current.Key;
            }
            return objlist;
        }

        private Brush[] AssembleFillBrushes(EventRecord rec, bool focus, bool selected, ref Color color)
        {
            if (selected)
                return new Brush[1] { new SolidBrush(color = Palette.getSelectionColor()) };
            else if (!focus && useOldStyle)
                return new Brush[1] { new SolidBrush(color = Palette.getTaskColor(rec.tid)) };
            else if (rec.mop == "TASK_FENCE" || rec.mop == "TRACED_EVENT")
                return new Brush[1] { new SolidBrush(color = Color.Black) };
            else if (rec.vars.Count > 0 && useOldStyle)
            {
                Brush[] brushes = new Brush[rec.vars.Count];
                int pos = 0;
                IEnumerator<KeyValuePair<int, int>> en = rec.vars.GetEnumerator();
                while (en.MoveNext())
                {
                    int var = en.Current.Key;
                    brushes[pos++] = new SolidBrush(color =
                         rec.isData ? Palette.getDataVarColor(var) :
                         (var < 256) ? Palette.getTaskColor(var) :
                         Palette.getSyncVarColor(var)
                    );
                }
                return brushes;
            }
            else
            {
                return new Brush[1] { new SolidBrush(color = useOldStyle ? Palette.getTaskColor(rec.tid) : Color.Gray) };
            }
        }

        private string AssembleText(EventRecord rec, bool blocked, string objlist)
        {
            string text;

            if (rec.mop == "TRACED_EVENT")
                return (useOldStyle ? "" + rec.tid + "." + rec.nr + ": " : "") + rec.name;

            if (useOldStyle)  // displays more detail... main purpose: our own debugging
            {
                text = "" + rec.tid + "." + rec.nr + ": "
                + (rec.name != null ? (rec.name + " ") : "")
                + ((rec.mop != null) ? (rec.mop + " ") : "")
                + objlist;
            }
            else
            {
                if (!String.IsNullOrEmpty(rec.name))
                    text = rec.name;
                else if (rec.mop != null)
                    text = rec.mop;
                else
                    text = objlist;
            }

            if (blocked)
                text = text + " (BLOCKS)";

            return text;
       }

        private Color AssembleTextColor(bool blocked, Color bgcolor)
        {
            if (useOldStyle)
                return (blocked ? Color.Crimson : bgcolor.GetBrightness() < 0.5 ? Color.White : Color.Black);
            else
                return blocked ? Color.Orange : Color.White;
        }

        internal Format GetFormat(Entry entry, bool focus, bool selected)
        {
            Format f = new Format();

            Color bgcolor = Color.Gray;
            bool blocked = (entry.status == "b");
            EventRecord rec = entry.record;

            lock (rec)
            {
                f.threadColor = Palette.getTaskColor(rec.tid);
                f.lineColor = (rec.boxColor != null) ? Color.FromName(rec.boxColor) : line;
                f.fatBorder = (rec.boxColor != null);
                f.fillBrushes = AssembleFillBrushes(rec, focus, selected, ref bgcolor);

                if (focus)
                {
                    f.objs = AssembleObjList(rec);
                    f.text = AssembleText(rec, blocked, f.objs);
                    f.textColor = AssembleTextColor(blocked, bgcolor);
                    f.font = textfont;
                }
                else
                {
                    f.cross = blocked;
                }

            }
            
            return f;
        }

        private IIndexConverter converter;
        internal IIndexConverter GetIndexConverter()
        {
            return converter;
        }

   
    }

    internal class Palette
    {

        private Palette()
        {
            // this class uses static members only... thus we include a private constructor to prevent instantiation
        }

        static internal Color getTaskColor(int inIndex)
        {
            return task_color[(uint)inIndex % task_color.Length];
        }
        static internal Color getSyncVarColor(int inIndex)
        {
            return syncvar_color[(uint)inIndex % syncvar_color.Length];
        }
        static internal Color getDataVarColor(int inIndex)
        {
            return datavar_color[(uint)inIndex % datavar_color.Length];
        }
        static internal Color getSelectionColor()
        {
            return Color.LightBlue;
        }

        

        static internal Color[] task_color = new Color[] {
			// for the tasks
            Color.MediumTurquoise,
			Color.BlueViolet,
			Color.CadetBlue,
            Color.CornflowerBlue,
			Color.DarkBlue,
	        Color.Blue,
		    Color.DeepSkyBlue,
		    Color.DodgerBlue,
		    Color.MediumSlateBlue,
		    Color.MidnightBlue,
		    Color.DarkSeaGreen,
		    Color.DarkSlateBlue,
		    Color.DarkSlateGray,
		    Color.DarkTurquoise,
		    Color.DarkViolet,
		    Color.Green,
		    Color.Indigo,
		    Color.MediumAquamarine,
		    Color.MediumBlue,
		    Color.MediumPurple,
		    Color.MediumSeaGreen,
		    Color.MediumSpringGreen,
		    Color.MediumTurquoise,
        };

        static internal Color[] syncvar_color = new Color[] {
		   Color.Brown,
		   Color.BurlyWood,
		   Color.Chocolate,
		   Color.Coral,
		   Color.Cornsilk,
		   Color.Crimson,
		   Color.DarkGoldenrod,
		   Color.DarkOrange,
		   Color.DarkOrchid,
		   Color.DarkRed,
		   Color.DarkSalmon,
		   Color.Firebrick,
		   Color.Gold,
		   Color.Goldenrod,
		   Color.IndianRed,
		   Color.Magenta,
		   Color.Maroon,
		   Color.MediumVioletRed,
		   Color.Orange,
		   Color.OrangeRed,
        };

        static internal Color[] datavar_color = new Color[] {
           Color.LightYellow,
           Color.LemonChiffon,
           Color.LightGoldenrodYellow,
           Color.PapayaWhip,
           Color.Honeydew,
           Color.MintCream,
           Color.Azure,
           Color.AliceBlue,
           Color.GhostWhite,
           Color.WhiteSmoke,
           Color.Beige,
           Color.AntiqueWhite,
           Color.Linen,
           Color.LavenderBlush,
           Color.MistyRose
       };

    };

}
