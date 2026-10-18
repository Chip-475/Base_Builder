using UnityEngine;
[CreateAssetMenu(fileName ="storage Data",menuName ="Building/storage")]
public class storageData : BuildingData
{
    [Min(1)] public int quant=100;
    void reset()
    {
        type=BuildingType.Storage;
        bounds=new Bounds(Vector3.zero,new Vector3(3,3,1));
        interactionBounds=new Bounds(Vector3.zero,new Vector3(5,5,1));
        connectionBounds=new Bounds(Vector3.zero,new Vector3(3,3,1));
        blocksWalking=true;
        blocksPlacing=true;
    }
}
