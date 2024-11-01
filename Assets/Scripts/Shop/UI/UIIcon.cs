using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIIcon : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer _inCartItemViewPrefab = default;
	 
	[SerializeField]
	private TMP_Text _text = default;
	private ShopCartUIManager _shopCartUIManager = default;
	private int _id;

	private void Update()
	{
		if(_id == 10001 && Input.GetKeyDown(KeyCode.Return))
		{
			Delete();
		}
	}

	public void Init(MeshFilter[] meshFilters,MeshRenderer[] meshRenderers,ShopCartUIManager shopCartUIManager,int id)
	{
		_id = id;
		_shopCartUIManager = shopCartUIManager;
		Transform myTransform = transform;

		for(int i = 0; i < meshFilters.Length ;i++)
		{
			MeshRenderer meshRenderer = Instantiate(_inCartItemViewPrefab,myTransform);
			meshRenderer.material = meshRenderers[i].sharedMaterial;
			meshRenderer.GetComponent<MeshFilter>().mesh = meshFilters[i].sharedMesh;
			Transform targetTransform = meshRenderers[i].transform;
			Transform viewTransform = meshRenderer.transform;
			viewTransform.position = targetTransform.localPosition;
			viewTransform.rotation = targetTransform.rotation;
			viewTransform.localScale = targetTransform.lossyScale;

		}
	}

	public void UpdateCount(int count)
	{
		if(count == 0)
		{
			Destroy(this.gameObject);
			return;
		}
		_text.SetText(count.ToString("x#"));
	}

	public void Delete()
	{
		_shopCartUIManager.DestoryCartUI(_id);
	}
}
