using CurseOfTheMoon.Content.NPCs.Town;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CurseOfTheMoon
{
	public class CurseOfTheMoon : Mod
	{
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			CotmMessageType msgType = (CotmMessageType)reader.ReadByte();

			switch (msgType)
			{
				default:
					Logger.WarnFormat("Cotm: Unknown Message type: {0}", msgType);
					break;
			}
		}
	}
	internal enum CotmMessageType : byte
	{
		ExamplePlayerSyncPlayer,
		ExampleTeleportToStatue
	}
}