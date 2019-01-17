using RAGE;
using RAGE.Game;
using RageMpClientHelpers;
using RageMpHelper;
using System.Collections.Generic;

namespace Timecycle
{
    public class TimecycleTester : Events.Script
    {
        private bool _on;
        private int _index = 1;

        public TimecycleTester()
        {
            GfxHelper.ClearCycle();
            Events.Tick += OnTick;

            _on = false;
        }

        private void OnTick(List<Events.TickNametagData> nametags)
        {
            if (Pad.IsControlJustPressed(0, (int)Control.ReplayStartStopRecordingSecondary))
            {
                UiHelper.ShowSubtitle("key pressed", 1000);
                if (!_on)
                {
                    _on = true;
                    GfxHelper.SetTimeCycleById(_index);
                }
                else
                {
                    GfxHelper.ClearCycle();
                    _on = false;
                }
            }

            if (Pad.IsControlJustPressed(0, (int)Control.PhoneLeft))
            {
                _index--;
                UpdateIndex();
            }
                
            else if (Pad.IsControlJustPressed(0, (int)Control.PhoneRight))
            {
                _index++;
                UpdateIndex();
            }                
        }

        private void UpdateIndex()
        {
            if (_index > 774)
                _index = 1;
            else if (_index < 1)
                _index = 774;

            UiHelper.ShowSubtitle($"Cycle: {GfxHelper.SetTimeCycleById(_index)}, ID:{_index}", 2500);
        }
    }
}
