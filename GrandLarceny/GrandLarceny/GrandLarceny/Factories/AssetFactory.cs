using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	public class AssetFactory
	{
		public static void createPlayer(Vector2 a_position)
		{
			States t_state = Game.getInstance().getState();
			t_state.setPlayer(new Player(t_state.getTileCoordinates(a_position), "Images//Sprite//Hero//hero_stand", 0.300f));
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

		public static void createDuckHidingObject(Vector2 a_position, string a_asset)
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
			t_state.addObject(new VentilationDrum(t_state.getTileCoordinates(a_position), a_asset, 0.700f));
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
			t_state.addObject(new GuardCamera(t_state.getTileCoordinates(a_position), a_asset, 0.200f, (float)(Math.PI * 0.5), (float)(Math.PI * 0.75), (float)(Math.PI * 0.25)));
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
			t_state.addObject(new Environment(t_state.getTileCoordinates(a_position), a_asset, 0.998f));
		}
	}
}
