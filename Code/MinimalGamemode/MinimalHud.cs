using Sandbox;
using Sandbox.UI;
namespace Advisor
{
	public partial class MinimalHudEntity : Hud
	{
		public MinimalHudEntity()
		{
			if ( IsClient )
			{
				RootPanel.SetTemplate( "MinimalGamemode/minimalhud.html" );
				RootPanel.AddChild<ChatBox>();
			}
		}
	}

}
