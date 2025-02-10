using System.Collections.Generic;

public class FloorWallView : DressUpViewBase
{
	protected override List<DressUpInformation> CreateDressUpInformation(in List<int> IDs)
	{
		List<DressUpInformation> floorWallInformation = new();
		foreach (var id in IDs)
		{
			if (AllItemBundleAsset.GetItemAssetByID(id).Genre == ItemGenre.Flooring)
			{
				floorWallInformation.Add(new DressUpInformation(0, id));
			}
			else if (AllItemBundleAsset.GetItemAssetByID(id).Genre == ItemGenre.Wallpaper)
			{
				floorWallInformation.Add(new DressUpInformation(1, id));
			}
		}

		return floorWallInformation;
	}
}
