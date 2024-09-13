using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingTarget : MonoBehaviour
{
    private const float GROUND_OFFSET = 0.01f;
    private const float FORWARD_OFFSET = 1.2f;

    private IEditOnlyGhost ghostModel = default;
    private Transform player = default;
    private Transform playerHead = default;
    private Collider[] hitResults = new Collider[4];
    private bool isCollision = default;
    private float yPosition = default;

    private void Awake()
    {
        // Tmporary
        playerHead = player.Find("CenterEyeAnchor");
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(player.position.x, yPosition, player.position.z) + player.forward * FORWARD_OFFSET;
        ghostModel.SetPlaceableState(PreviewPlacing());
    }

    public PlacingTarget Initialize(IEditOnlyGhost ghostModel, Transform player)
    {
        this.ghostModel = ghostModel;
        this.player = player;
        return this;
    }

    private bool PreviewPlacing()
    {
        const float RAY_LIMIT = 2f;

        Ray toWallRay = new Ray(playerHead.position, playerHead.forward);
        bool isHitFront = Physics.Raycast(toWallRay, out RaycastHit wallHitInfo, FORWARD_OFFSET, Layer.GROUNDWALL);
        Debug.DrawRay(toWallRay.origin, toWallRay.direction * FORWARD_OFFSET, Color.blue);
        if (isHitFront && wallHitInfo.normal.y == 0f && wallHitInfo.collider.bounds.extents.y < 2f)
        {
            // top half
            if (wallHitInfo.point.y > wallHitInfo.transform.position.y)
            {

            }
            // bottom half
            else
            {

            }
        }
        else
        {

        }

        Ray toGroundRay = new Ray(transform.position, Vector3.down);
        bool isHitGround = Physics.Raycast(toGroundRay, out RaycastHit groundHitInfo, RAY_LIMIT, Layer.GROUNDWALL);

        if (!isHitGround)
        {
            return false;
        }
        if (groundHitInfo.normal != Vector3.up)
        {
            return false;
        }

        yPosition = groundHitInfo.point.y + GROUND_OFFSET;
        if (isCollision)
        {
            return false;
        }

        Vector3 toPlayer = player.position - transform.position;
        Ray toPlayerRay = new Ray(transform.position, toPlayer.normalized);
        bool isHitBack = Physics.Raycast(toPlayerRay, out RaycastHit _, toPlayer.magnitude, Layer.GROUNDWALL);
        if (isHitBack)
        {
            return false;
        }
        // プレイヤーの頭の向きにRayを飛ばし、WarpPointerの要領で地面か机の上かを判定する。
        // 机のコライダーの上半分にあたったら机の上だし、下半分に当たったら地面。
        // あまりにもその壁が高かったら柱と判断し、return

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        isCollision = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isCollision = false;
    }
}
