/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/


using Microsoft.Concurrency.TestTools.Execution;
using System;
namespace Microsoft.Concurrency.TestTools.UnitTesting.MCutConsole
{
    class Model : IEntityModel
    {

        #region Static Members

        private static Model _singleton;
        public static Model Instance
        {
            get
            {
                if (_singleton == null)
                    _singleton = new Model();

                return _singleton;
            }
        }

        #endregion

        internal Model()
        {
            this.EntityBuilder = new EntityBuilder(this);
        }

        public EntityBuilderBase EntityBuilder { get; private set; }

        public SessionEntity Session { get; private set; }

        ISessionEntity IEntityModel.Session { get { return Session; } }

        internal void InitializeSession(string sessionFilePath)
        {
            Session = new SessionEntity(sessionFilePath);
        }

    }
}
