using RAGE;
using RAGE.Game;
using RageMpClientHelpers;
using System.Collections.Generic;

namespace VehicleDial
{
    public class VehicleDial : Events.Script
    {
        /// <summary>
        /// Do nothing if disabled
        /// </summary>
        private bool ScriptEnabled = true;

        private RAGE.Elements.Player _ply;

        #region Dashboard Fields
        Scaleform _dashBoard = null;
        private System.Drawing.Point _screenRes = new System.Drawing.Point();
        private System.Drawing.Point _dialPosition = new System.Drawing.Point();
        private System.Drawing.Point _dialSize = new System.Drawing.Point(); 
        #endregion

        public VehicleDial()
        {
            _ply = RAGE.Elements.Player.LocalPlayer;
            
            if (ScriptEnabled)
            {
                _dashBoard = ScaleformHelper.Dashboard();

                Events.Tick += OnTick;
            }            
        }

        private void OnTick(List<Events.TickNametagData> nametags)
        {
            var veh = _ply.Vehicle;
            if (_ply.Vehicle != null)
            {
                
                UpdateScreenRes();

                var sizeX = _screenRes.X / 3;
                var sizeY = _screenRes.Y / 3;


                _dialPosition.X = (_screenRes.X - sizeX);
                _dialPosition.Y = (_screenRes.Y - sizeY) - 30;

                _dialSize.X = sizeX;
                _dialSize.Y = sizeY;

                _dashBoard.Render2DScreenSpace(_dialPosition, _dialSize);
            }
        }

        private void UpdateScreenRes()
        {
            int x = 0; int y = 0;
            RAGE.Game.Graphics.GetScreenResolution(ref x, ref y);
            _screenRes.X = x;_screenRes.Y = y;
        }
    }
}
