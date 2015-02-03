using Sce.PlayStation.Core;

namespace TheATeam
{
	public class HealthWall: EntityAlive
	{
		public HealthWall(char key, Vector2 position): base(position)
		{
			Key = key;
			Stats.Health = 1;
			Stats.Lives = 3;
			Stats.MaxHealth = 1;
			Stats.MaxLives = 3;
		}
		
		override public void Update(float dt)
		{
			if(IsAlive)
			{
				TileIndex2D.X = TileIndex2D.X + Stats.MaxLives - Stats.Lives;
			}
			else
			{
				Key = 'E';
				UnscheduleUpdate();
			}
			base.Update(dt);
		}
	}
}