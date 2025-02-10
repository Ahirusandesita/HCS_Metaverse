using System.Collections.Generic;
using System.Linq;

public class FloorWallView : DressUpViewBase
{
	protected override List<DressUpInformation> CreateDressUpInformation(in List<int> IDs)
	{
		List<DressUpInformation> floorWallInformation = new();
		floorWallInformation.Add(new DressUpInformation(0, -1, "Floor"));
		floorWallInformation.Add(new DressUpInformation(1, -1, "Wall"));
		foreach (var id in IDs)
		{
			if (AllItemBundleAsset.GetItemAssetByID(id).Genre == ItemGenre.Flooring)
			{
				floorWallInformation.Add(new DressUpInformation(0, id, "Floor"));
			}
			else if (AllItemBundleAsset.GetItemAssetByID(id).Genre == ItemGenre.Wallpaper)
			{
				floorWallInformation.Add(new DressUpInformation(1, id, "Wall"));
			}
		}

		return floorWallInformation;
	}
}
