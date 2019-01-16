using System;
using Newtonsoft.Json;
using RAGE;
using RAGE.Elements;
using RageMpClientHelpers;

namespace XpLevel
{
    public class XpLevel : Events.Script
    {
        /// <summary>
        /// Client variable name, server is named different
        /// </summary>
        const string SET_LEVEL = "SET_LEVEL";
        const ulong  SET_LEVEL_VAL = 4930075823048855057;

        /// <summary>
        /// Enables and listens for Entity DataChanged events from server
        /// </summary>
        public XpLevel()
        {
            Events.EnableKeyDataChangeEvent = true;
            Events.OnEntityDataChangeByKey += DataChanged;
        }

        /// <summary>
        /// Displays level information when data changes
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        /// <param name="arg"></param>
        private void DataChanged(ulong key, Entity entity, object arg)
        {
            ///This variable name should be unknown to the client
            if (key == SET_LEVEL_VAL)
            {
                entity.SetData(SET_LEVEL, arg);

                PlayerHelper.DisplayLevelUpScaleform(
                    JsonConvert.DeserializeObject<int[]>(arg.ToString()));
            }
        }
    }
}
