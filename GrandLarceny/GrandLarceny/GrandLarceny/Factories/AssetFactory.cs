using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	public class AssetFactory
	{
		public static void createPlayer(Vector2 a_position)
		{
			if (Game.getInstance().getState().getPlayer() == null) {
				States t_state = Game.getInstance().getState();
				Player t_player = new Player(t_state.getTileCoordinates(a_position), "Images//Sprite//Hero//hero_stand", 0.300f);
				t_state.setPlayer(t_player);
				t_state.addObject(t_player);
			}
		}

		public static void createPlatform(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new Platform(t_state.getTileCoordinates(a_position), a_asset, 0.350f));
		}

		public static void createLadder(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new Ladder(t_state.getTileCoordinates(a_position), a_asset, 0.350f));
		}

		public static void createSpotLight(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new SpotLight(t_state.getTileCoordinates(a_position), a_asset, 0.200f, (float)(Math.PI * 0.5f), true));
		}

		public static void createBackground(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new Environment(t_state.getTileCoordinates(a_position), a_asset, 0.999f));
		}

		public static void createGuard(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new Guard(t_state.getTileCoordinates(a_position), a_asset, t_state.getTileCoordinates(a_position).X, true, 0.250f));
		}

		public static void createWall(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new Wall(t_state.getTileCoordinates(a_position), a_asset, 0.350f));
		}

		public static void createDuckHideObject(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new DuckHideObject(t_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createStandHideObject(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new StandHideObject(t_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createGuardDog(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			Vector2 t_worldMouse = a_position;
			t_state.addObject(new GuardDog(t_state.getTileCoordinates(t_worldMouse), a_asset, t_state.getTileCoordinates(t_worldMouse).X, t_state.getTileCoordinates(t_worldMouse).X, 0.299f));
		}

		public static void createLightSwitch(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new LampSwitch(t_state.getTileCoordinates(a_position), a_asset, 0.750f));
		}

		public static void createCrossVent(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new CrossVentilation(t_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createTVent(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new TVentilation(t_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createStraightVent(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new StraightVentilation(t_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createCornerVent(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new CornerVentilation(t_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createVentrance(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			VentilationDrum t_outsideVentrance = new VentilationDrum(t_state.getTileCoordinates(a_position), a_asset, 0.699f);
			VentilationDrum t_insideVentrance = new VentilationDrum(t_state.getTileCoordinates(a_position), a_asset, 0.699f);
			t_outsideVentrance.setPairedVentilation(t_insideVentrance);
			t_insideVentrance.setPairedVentilation(t_outsideVentrance);
			t_state.addObject(t_outsideVentrance, 0);
			t_state.addObject(t_insideVentrance, 1);
		}

		public static void createVentEnd(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new VentilationEnd(t_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createForeground(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new Foreground(t_state.getTileCoordinates(a_position), a_asset, 0.100f));
		}

		public static void createRope(Vector2 a_position)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new Rope(t_state.getTileCoordinates(a_position) + new Vector2(36, 0), null, 0.100f));
		}

		public static void createCamera(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new GuardCamera(t_state.getTileCoordinates(a_position), a_asset, 0.200f, (float)(Math.PI * 0.5), (float)(Math.PI * 0.75), (float)(Math.PI * 0.25), true));
		}

		public static void createWindow(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new Window(t_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createSecDoor(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new SecurityDoor(t_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createCornerHang(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new CornerHang(t_state.getTileCoordinates(a_position), a_asset, 0.400f, 0.0f));
		}

		public static void createCheckPoint(Vector2 a_position)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new CheckPoint(t_state.getTileCoordinates(a_position), "Images//Tile//1x1_tile_ph", 0.200f, 0.0f));
		}

		public static void createProp(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			if (KeyboardHandler.isKeyPressed(Keys.LeftShift) || KeyboardHandler.isKeyPressed(Keys.RightShift))
			{
				t_state.addObject(new Environment(a_position, a_asset, 0.998f));
			}
			else
			{
				t_state.addObject(new Environment(t_state.getTileCoordinates(a_position), a_asset, 0.998f));
			}
		}

		public static void createKey(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new ConsumableKey(t_state.getTileCoordinates(a_position), a_asset, 0.250f));
		}

		public static void createHeart(Vector2 a_position, string a_asset)
		{
			States t_state = Game.getInstance().getState();
			t_state.addObject(new ConsumableHeart(t_state.getTileCoordinates(a_position), a_asset, 0.250f));
		}

		public static void copyAsset(Vector2 a_position, GameObject a_asset)
		{
			States t_state = Game.getInstance().getState();
			Vector2 t_position;
			if (a_position == null) {
				t_position = a_asset.getPosition().getGlobalCartesian();
			} else {
				t_position = a_position;
			}
			string t_imagePath = a_asset.getImg().getImagePath();

			if (a_asset is Player) {
				createPlayer(t_position);
			}
			if (a_asset is Platform) {
				createPlatform(t_position, t_imagePath);
			}
			if (a_asset is Ladder) {
				createLadder(t_position, t_imagePath);
			}
			if (a_asset is SpotLight) {
				createSpotLight(t_position, t_imagePath);
			}
			if (a_asset is Environment) {
				createBackground(t_position, t_imagePath);
			}
			if (a_asset is Guard) {
				createGuard(t_position, t_imagePath);
			}
			if (a_asset is Wall) {
				createWall(t_position, t_imagePath);
			}
			if (a_asset is DuckHideObject) {
				createDuckHideObject(t_position, t_imagePath);
			}
			if (a_asset is StandHideObject) {
				createStandHideObject(t_position, t_imagePath);
			}
			if (a_asset is GuardDog) {
				createGuardDog(t_position, t_imagePath);
			}
			if (a_asset is LampSwitch) {
				createLightSwitch(t_position, t_imagePath);
			}
			if (a_asset is CrossVentilation) {
				createCrossVent(t_position, t_imagePath);
			}
			if (a_asset is TVentilation) {
				createTVent(t_position, t_imagePath);
			}
			if (a_asset is StraightVentilation) {
				createStraightVent(t_position, t_imagePath);
			}
			if (a_asset is CornerVentilation) {
				createCornerVent(t_position, t_imagePath);
			}
			if (a_asset is VentilationDrum) {
				createVentrance(t_position, t_imagePath);
			}
			if (a_asset is Foreground) {
				createForeground(t_position, t_imagePath);
			}
			if (a_asset is Rope) {
				createRope(t_position);
			}
			if (a_asset is GuardCamera) {
				createCamera(t_position, t_imagePath);
			}
			if (a_asset is Window) {
				createWindow(t_position, t_imagePath);
			}
			if (a_asset is SecurityDoor) {
				createSecDoor(t_position, t_imagePath);
			}
			if (a_asset is CornerHang) {
				createCornerHang(t_position, t_imagePath);
			}
			if (a_asset is CheckPoint) {
				createCheckPoint(t_position);
			}
		}
	}
}
