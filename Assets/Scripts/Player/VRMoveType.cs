
/// <summary>
/// VRのプレイヤーの移動方式
/// </summary>
public enum VRMoveType
{
    /// <summary>
    /// スタンダードでナチュラルな移動方式。スティックを倒し任意の方向に連続的に移動することが可能。
    /// </summary>
    Natural,
    /// <summary>
    /// 酔い対策に効果的なワープ方式。スティックを倒している間ワープ先が表示され、離すと指定した位置に瞬間的に移動。
    /// </summary>
    Warp,
}

/// <summary>
/// VRのプレイヤーの転回方式
/// </summary>
public enum VRRotateType
{
    /// <summary>
    /// スムーズな転回方式。スティックを倒した方向に連続的に転回する。（TunnelingVignetの有効化をおすすめ）
    /// </summary>
    Analog,
    /// <summary>
    /// 酔い対策に有効な転回方式。スティックを倒した方向に離散的に転回する。
    /// </summary>
    Degital,
}