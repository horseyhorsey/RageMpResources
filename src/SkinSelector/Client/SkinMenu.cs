using System;
using System.Collections.Generic;
using System.Linq;
using GTAN.Data;
using RAGE;
using RAGE.Game;
using RAGE.NUI;
using RageMpClientBase;
using RageMpClientHelpers;

namespace SkinSelectorClient
{
    /// <summary>
    /// Exposes event "ShowCharacterSelection" to activate the menu
    /// </summary>
    public class SkinMenu : MenuBaseScript
    {
        #region Properties
        /// <summary>
        /// Point at the player when open menu
        /// </summary>
        private int _camera;
        /// <summary>
        /// Are we using male freemode
        /// </summary>
        private bool _maleSelected = true;
        /// <summary>
        /// The player running this script
        /// </summary>
        private RAGE.Elements.Player _ply;                
        /// <summary>
        /// Outfits. 0 - 1483
        /// </summary>
        private UIMenuListItem _outfits;        
        /// <summary>
        /// Selected MP outfit
        /// </summary>
        private int selectedOutfit = 0;
        /// <summary>
        /// Set some chat messages to only show debug.
        /// </summary>
        private bool _debug = true;
        /// <summary>
        /// Selected model when selected
        /// </summary>
        uint SelectedModelHash { get; set; }
        #endregion

        #region Constructors
        public SkinMenu()
        {                        
            _ply = RAGE.Elements.Player.LocalPlayer;

            //Init Menu
            CreateMainMenu("Main Menu", "Skin Selection");
            InitializePedTypesList();
            MainMenu.OnMenuChange += MainMenu_OnMenuChange;
            RefreshIndex();

            //Show character selection when activated from server or local
            Events.Add("ShowSkinSelector", OnShowSkinSelector);

            if (_debug)
                Chat.Output("Skin selector client loaded | debug");
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Draws menu to screen, if any are open. Activated with interaction menu
        /// </summary>
        protected override void DrawMenu(List<Events.TickNametagData> nametags)
        {
            if (Pad.IsControlJustPressed(0, (int)Control.InteractionMenu))
            {
                if (_debug)
                    Chat.Output("Player just pressed interactive key");

                if (!_ply.IsSittingInAnyVehicle())
                {
                    if (!MenuPool.IsAnyMenuOpen())
                    {
                        OnShowSkinSelector(null);
                    }
                }
                else
                    Chat.Output("Try outside of the vehicle");
            }

            if (MenuPool.IsAnyMenuOpen())
            {
                MenuPool.ProcessMenus();
            }
        }

        /// <summary>
        /// Menu closing sends "CharacterChanged (SelectedModelHash)" to server
        /// </summary>
        /// <param name="sender"></param>
        protected override void MainMenu_OnMenuClose(UIMenu sender)
        {
            //Destroy cameras, set this _camera to inactive
            Cam.RenderScriptCams(false, true, 0, false, false, 0);
            Cam.SetCamActive(_camera, false);
            Cam.DestroyCam(_camera, false);

            //Enable the players controls again
            RAGE.Elements.Player.LocalPlayer.SetGravity(true);
            Pad.EnableAllControlActions(0);
            
            //Let server know menu is closed to change if selected a ped to sync.
            RAGE.Events.CallRemote("ClientMenuClosed", SelectedModelHash.ToString(), selectedOutfit);

            PlayerHelper.FreezePlayer(false, false);
            
            ChatHelper.EnableChat(true);
            UiHelper.EnableHuds();

            if (_debug)
                Chat.Output("Skin selector Menu closed");
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a dictionary from the PEDS class
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static Dictionary<string, uint> GetPedDict(string name)
        {
            var pedType = typeof(Peds);
            var pedDict = pedType.GetProperty(name);
            Dictionary<string, uint> peds = pedDict.GetValue(pedDict) as Dictionary<string, uint>;
            return peds;
        }

        /// <summary>
        /// Gets the names of all the Dictionaries in <see cref="Peds"/> and builds NATIVEUI menu items for main menu list
        /// </summary>
        private void InitializePedTypesList()
        {
            //out fit selectors male / female
            if (_debug)
                Chat.Output("init outfit dicts");            
            var maleMp = AddSubMenu(MainMenu, "Male", "Custom");
            var femaleMp = AddSubMenu(MainMenu, "Female", "Custom");
            var numArray = new object[1483];
            for (int i = 0; i < numArray.Length; i++)
            {
                numArray[i] = i;
            }
            _outfits = new UIMenuListItem("Outfits", numArray.ToList(), 0);
            _outfits.Activated += Outfits_Activated;
            maleMp.AddItem(_outfits);
            femaleMp.AddItem(_outfits);

            //Ped categories
            if (_debug)
                Chat.Output("init ped lists");
            var type = typeof(Peds);
            var properties = type.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (var prop in properties)
            {
                var propName = prop.Name;

                var menu = AddSubMenu(MainMenu, propName, propName + " Skins");
                menu.OnIndexChange += PedLists_OnIndexChange;
            }

            MainMenu.OnItemSelect += MainMenu_OnItemSelect;
        }

        /// <summary>
        /// Sets the appropriate model if free mode skin selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedItem"></param>
        /// <param name="index"></param>
        private void MainMenu_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (_debug)
                Chat.Output($"item selected: {selectedItem.Text}");

            if (selectedItem.Text == "Male")
            {
                _maleSelected = true;
                _ply.Model = 1885233650;
                SelectedModelHash = _ply.Model;
            }
            else if (selectedItem.Text == "Female")
            {
                _maleSelected = false;
                _ply.Model = 2627665880;
                SelectedModelHash = _ply.Model;
            }
        }

        /// <summary>
        /// Requests server change the outfit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedItem"></param>
        private void Outfits_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            selectedOutfit = _outfits.Index;

            if (_debug)
                Chat.Output($"requesting ChangeOutfit {selectedOutfit}");

            Events.CallRemote("ChangeOutfit", _outfits.Index, _maleSelected ? 0 : 1);            
        }

        /// <summary>
        /// Adds menu items from selected dictionary if not in the main menu SOTW
        /// </summary>
        /// <param name="oldMenu"></param>
        /// <param name="newMenu"></param>
        /// <param name="forward"></param>
        private void MainMenu_OnMenuChange(UIMenu oldMenu, UIMenu newMenu, bool forward)
        {
            if (_debug)
                Chat.Output($"Main menu changed: {newMenu.Title.Caption}");

            var menuCaption = newMenu.Subtitle.Caption;
            if (menuCaption.ToUpper() != "MAIN MENU")
            {
                if (_debug)
                    Chat.Output($"populating peds: {menuCaption}. ItemsExist:{newMenu.MenuItems.Count}");

                Dictionary<string, uint> peds = GetPedDict(menuCaption);
                foreach (var skin in peds)
                {
                    var menuItem = new UIMenuItem(skin.Key, skin.Value.ToString());
                    newMenu.AddItem(menuItem);
                }
                return;
            }
        }

        /// <summary>
        /// Places the <see cref="_camera"/> in front of the player
        /// </summary>
        /// <param name="args">NULL</param>
        private void OnShowSkinSelector(object[] args)
        {
            if (_debug)
                Chat.Output("Skin selector opening");

            Vector3 position = new Vector3(_ply.Position.X, _ply.Position.Y, _ply.Position.Z);
            var ply = RAGE.Elements.Player.LocalPlayer;
            ply.Position = position;

            ply.SetHeading(343f);

            UiHelper.EnableHuds(false, false);

            ply.SetGravity(true);

            if (!_debug)
               ChatHelper.EnableChat(false);

            var heading = ply.GetHeading();
            Vector3 offset = RAGE.Game.Entity.GetOffsetFromEntityInWorldCoords(ply.Handle, 1f, 2f, 0f);

            _camera = Cam
                .CreateCameraWithParams(Misc.GetHashKey("DEFAULT_SCRIPTED_CAMERA"), offset.X, offset.Y, offset.Z, 0f, 0f, 171.5f, 50f, true, 2);
            Cam.SetCamActive(_camera, true);
            Cam.RenderScriptCams(true, true, 0, false, false, 0);
            Cam.PointCamAtCoord(_camera, ply.Position.X, ply.Position.Y, ply.Position.Z);

            PlayerHelper.FreezePlayer(true, false);
            MainMenu.Visible = true;

            if (_debug)
                Chat.Output("Skin selector opened");
        }

        /// <summary>
        /// Sets the <see cref="SelectedModelHash"/> each time an item is selected in the menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="newIndex"></param>
        private void PedLists_OnIndexChange(UIMenu sender, int newIndex)
        {
            if(_debug)
                Chat.Output($"ped index changed, new index: {newIndex}");

            var item = sender.MenuItems[newIndex];

            uint modelHash = Convert.ToUInt32(item.Description);
            _ply.Model = (uint)modelHash;
            SelectedModelHash = modelHash;

            if (_debug)
                Chat.Output($"selected ped hash: {SelectedModelHash}");
        }

        #endregion
    }
}