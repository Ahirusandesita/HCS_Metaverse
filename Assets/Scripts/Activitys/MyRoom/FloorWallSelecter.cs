using UnityEngine;

public class FloorWallSelecter : MonoBehaviour, IDressUpEventSubscriber
{
	[SerializeField] private ItemBundleAsset allItemAsset = default;
	[SerializeField] private MeshRenderer[] floorRenderers = default;
	[SerializeField] private MeshRenderer[] wallRenderers = default;

	void IDressUpEventSubscriber.OnDressUp(int id, string name)
	{
		if (name == "Floor")
		{

		}
		else
		{

		}

		// 選択が初期値（-1）の場合
		if (name == "Wall")
		{
			foreach (var wall in wallRenderers)
			{
				wall.enabled = false;
			}
		}
		else if (!wallRenderers[0].enabled)
		{
			foreach (var wall in wallRenderers)
			{
				wall.enabled = true;
			}
		}

		ItemAsset itemAsset = allItemAsset.GetItemAssetByID(id);
		if (itemAsset.Genre == ItemGenre.Flooring)
		{
			foreach (var mr in floorRenderers)
			{
				mr.sharedMaterial = itemAsset.Material;
			}
		}
		else if (itemAsset.Genre == ItemGenre.Wallpaper)
		{
			foreach (var mr in wallRenderers)
			{
				mr.sharedMaterial = itemAsset.Material;
			}
		}
	}
}
