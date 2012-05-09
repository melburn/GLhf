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
		private static States m_state;

		public static void updateState(States a_currentState)
		{
			m_state = a_currentState;
		}

		public static void createPlayer(Vector2 a_position)
		{
			if (Game.getInstance().getState().getPlayer() == null) {
				m_state = Game.getInstance().getState();
				Player t_player = new Player(m_state.getTileCoordinates(a_position), "Images//Sprite//Hero//hero_stand", 0.300f);
				m_state.setPlayer(t_player);
				m_state.addObject(t_player);
			}
		}

		public static void createPlatform(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new Platform(m_state.getTileCoordinates(a_position), a_asset, 0.350f));
		}

		public static void createLadder(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new Ladder(m_state.getTileCoordinates(a_position), a_asset, 0.350f));
		}

		public static void createSpotLight(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new SpotLight(m_state.getTileCoordinates(a_position), a_asset, 0.200f, (float)(Math.PI * 0.5f), true));
		}

		public static void createBackground(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new Environment(m_state.getTileCoordinates(a_position), a_asset, 0.950f));
		}

		public static void createGuard(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new Guard(m_state.getTileCoordinates(a_position), a_asset, m_state.getTileCoordinates(a_position).X, true, 0.250f));
		}

		public static void createWall(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new Wall(m_state.getTileCoordinates(a_position), a_asset, 0.350f));
		}

		public static void createDuckHideObject(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new DuckHideObject(m_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createStandHideObject(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new StandHideObject(m_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createGuardDog(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			Vector2 t_worldMouse = a_position;
			m_state.addObject(new GuardDog(m_state.getTileCoordinates(t_worldMouse), a_asset, m_state.getTileCoordinates(t_worldMouse).X, m_state.getTileCoordinates(t_worldMouse).X, 0.299f));
		}

		public static void createLightSwitch(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new LampSwitch(m_state.getTileCoordinates(a_position), a_asset, 0.750f));
		}

		public static void createCrossVent(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new CrossVentilation(m_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createTVent(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new TVentilation(m_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createStraightVent(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new StraightVentilation(m_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createCornerVent(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new CornerVentilation(m_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createVentrance(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			VentilationDrum t_outsideVentrance = new VentilationDrum(m_state.getTileCoordinates(a_position), a_asset, 0.699f);
			VentilationDrum t_insideVentrance = new VentilationDrum(m_state.getTileCoordinates(a_position), a_asset, 0.699f);
			t_outsideVentrance.setPairedVentilation(t_insideVentrance);
			t_insideVentrance.setPairedVentilation(t_outsideVentrance);
			m_state.addObject(t_outsideVentrance, 0);
			m_state.addObject(t_insideVentrance, 1);
		}

		public static void createVentEnd(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new VentilationEnd(m_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createForeground(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new Foreground(m_state.getTileCoordinates(a_position), a_asset, 0.100f));
		}

		public static void createRope(Vector2 a_position)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new Rope(m_state.getTileCoordinates(a_position) + new Vector2(36, 0), null, 0.150f));
		}

		public static void createCamera(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new GuardCamera(m_state.getTileCoordinates(a_position), a_asset, 0.200f, (float)(Math.PI * 0.5), (float)(Math.PI * 0.75), (float)(Math.PI * 0.25), true));
		}

		public static void createWindow(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new Window(m_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createSecDoor(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new SecurityDoor(m_state.getTileCoordinates(a_position), a_asset, 0.700f));
		}

		public static void createCornerHang(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new CornerHang(m_state.getTileCoordinates(a_position), a_asset, 0.400f, 0.0f));
		}

		public static void createCheckPoint(Vector2 a_position)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new CheckPoint(m_state.getTileCoordinates(a_position), "Images//Tile//1x1_tile_ph", 0.200f, 0.0f));
		}

		public static void createProp(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			if (KeyboardHandler.isKeyPressed(Keys.LeftShift) || KeyboardHandler.isKeyPressed(Keys.RightShift))
			{
				m_state.addObject(new Environment(a_position, a_asset, 0.949f));
			}
			else
			{
				m_state.addObject(new Environment(m_state.getTileCoordinates(a_position), a_asset, 0.949f));
			}
		}

		public static void createKey(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new ConsumableKey(m_state.getTileCoordinates(a_position), a_asset, 0.250f));
		}

		public static void createHeart(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new ConsumableHeart(m_state.getTileCoordinates(a_position), a_asset, 0.250f));
		}

		public static void createObjective(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new ConsumableGoal(m_state.getTileCoordinates(a_position), a_asset, 0.250f));
		}

		public static void createClosedDoor(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new LockedDoor(m_state.getTileCoordinates(a_position), a_asset, 0.400f));
		}

		public static void createCoveringShadow(Vector2 a_position, string a_asset)
		{
			m_state = Game.getInstance().getState();
			m_state.addObject(new CoveringShadow(m_state.getTileCoordinates(a_position), a_asset, 0.101f));
		}

		public static void copyAsset(Vector2 a_position, GameObject a_asset)
		{
			m_state = Game.getInstance().getState();
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
			if (a_asset is ConsumableGoal) {
				createObjective(t_position, t_imagePath);
			}
			if (a_asset is ConsumableKey) {
				createKey(t_position, t_imagePath);
			}
			if (a_asset is ConsumableHeart) {
				createHeart(t_position, t_imagePath);
			}
			if (a_asset is LockedDoor) {
				createClosedDoor(t_position, t_imagePath);
			}
			if (a_asset is CoveringShadow) {
				createCoveringShadow(t_position, t_imagePath);
			}
		}
	}
}
