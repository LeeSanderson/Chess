using Microsoft.Concurrency.TestTools.Execution;
using Microsoft.Concurrency.TestTools.UnitTesting.Xml;
using Microsoft.Concurrency.TestTools.Execution.Xml;

namespace Microsoft.Concurrency.TestTools.Alpaca.Aspects
{
    /// <summary>Builder class for creating entity instances.</summary>
    internal class EntityBuilder : EntityBuilderBase
    {

        internal EntityBuilder(Model model)
            : base(model)
        {
        }

        protected override void OnRegisterKnownEntities()
        {
            base.OnRegisterKnownEntities();

            RegisterXEntityFactory(XSessionNames.Session, x => new SessionEntity(x));
            RegisterXEntityFactory(XSessionNames.SessionState, x => new SessionRuntimeState(x));

            //RegisterXEntityFactory(XSessionNames.Pluginengine, x => new PluginEngineEntity(x));
        }

    }
}
