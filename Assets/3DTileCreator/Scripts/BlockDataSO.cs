using UnityEngine;
namespace TilePainter3D.Blocks
{
    [CreateAssetMenu(menuName = "TilePainter3D/BlockData")]
    public class BlockDataSO : ScriptableObject
    {
        [SerializeField]
        public
        BlockData blockData;

    }
}