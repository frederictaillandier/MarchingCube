using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class March
{
    public string reference;
    public List<Vector3> vertices;
    public List<int> tris;
    public List<Vector2> uvs;
}


public class MarchingCubeLibrary : MonoBehaviour
{
    public List<March> marches;
    static public List<March> Marches;

    private void Awake()
    {
        Marches = marches;
        // Todo : Export to an Editor Object
        //CheckAndCleanLibrary();
    }

    public static March GetMarch(uint reference)
    {
        return Marches[(int)reference];
    }

    #region autoGeneration and fix Corruption


    /*
        //This whole part is probably not optimized, but I don't care, as it's helpers to re-build the Marching cube Library
        //Should only be readable

        //Searching for the march looking by reference
        public static March GetMarch(uint reference)
        {
            uint referenceBack = reference;
            string strRef = "";


            for (int i = 0; i < 8; ++i)
            {
                if (i == 4)
                    strRef = "-" + strRef;

                string bit = ((reference & 1) == 1) ? "1" : "0";
                strRef = bit + strRef;
                reference = reference >> 1;
            }
            for (int i = 0; i < Marches.Count; ++i)
            {
                if (Marches[i].reference == strRef)
                {
                    if (referenceBack != i)
                        Debug.LogWarning("Marching Cube Library: item " + strRef + " at position " + i.ToString() + " when it should be " + referenceBack.ToString());
                    return Marches[i];
                }

            }
            Debug.LogError("Missing cube in marching cube library reference : " + strRef);
            return null;
        }

        public class Comp : IComparer<March>
        {
            public int Compare(March x, March y)
            {
                return string.Compare(x.reference, y.reference);
            }
        }

        private void GenerateNegatives() // if 11110000 exist, create its invert 00001111 
        {
            for (uint i = 0; i < 256; ++i)
            {
                if (GetMarch(i) == null && GetMarch((~i) % 256) != null)
                {

                    March toCopy = GetMarch((~i) % 256);
                    March newMarch = new March();
                    newMarch.vertices = new List<Vector3>(toCopy.vertices);
                    newMarch.reference = "";
                    foreach (var c in toCopy.reference)
                    {
                        if (c == '1')
                            newMarch.reference += '0';
                        else if (c == '0')
                            newMarch.reference += '1';
                        else
                            newMarch.reference += c;
                    }
                    newMarch.tris = new List<int>();
                    for (int j = 0; j < toCopy.tris.Count; j += 3)
                    {
                        newMarch.tris.Add(toCopy.tris[j]);
                        newMarch.tris.Add(toCopy.tris[j + 2]);
                        newMarch.tris.Add(toCopy.tris[j + 1]);
                    }
                    marches.Add(newMarch);

                }
            }
        }

        //convert reference into list vector3 to allow transformations
        private List<Vector3> ConvertReferenceToVectorList(uint reference)
        {
            List<Vector3> result = new List<Vector3>();

            if ((reference & (1 << 7)) != 0)
                result.Add(new Vector3(0, 0, 0));
            if ((reference & (1 << 6)) != 0)
                result.Add(new Vector3(1, 0, 0));
            if ((reference & (1 << 5)) != 0)
                result.Add(new Vector3(0, 0, 1));
            if ((reference & (1 << 4)) != 0)
                result.Add(new Vector3(1, 0, 1));
            if ((reference & (1 << 3)) != 0)
                result.Add(new Vector3(0, 1, 0));
            if ((reference & (1 << 2)) != 0)
                result.Add(new Vector3(1, 1, 0));
            if ((reference & (1 << 1)) != 0)
                result.Add(new Vector3(0, 1, 1));
            if ((reference & (1 << 0)) != 0)
                result.Add(new Vector3(1, 1, 1));
            return result;
        }

        //convert list vector3 into reference once transformation is done
        private uint ConvertVectorListToReference(List<Vector3> list)
        {
            uint result = 0;

            foreach (Vector3 v in list)
            {
                if (v == new Vector3(0, 0, 0))
                    result = result | 1 << 7;
                else if (v == new Vector3(1, 0, 0))
                    result = result | 1 << 6;
                else if (v == new Vector3(0, 0, 1))
                    result = result | 1 << 5;
                else if (v == new Vector3(1, 0, 1))
                    result = result | 1 << 4;
                else if (v == new Vector3(0, 1, 0))
                    result = result | 1 << 3;
                else if (v == new Vector3(1, 1, 0))
                    result = result | 1 << 2;
                else if (v == new Vector3(0, 1, 1))
                    result = result | 1 << 1;
                else if (v == new Vector3(1, 1, 1))
                    result = result | 1 << 0;
            }
            return result;
        }


        private uint Rotation(uint reference, Quaternion q)
        {
            List<Vector3> listedRef = ConvertReferenceToVectorList(reference);

            for (int i = 0; i < listedRef.Count; ++i)
            {
                listedRef[i] -= (Vector3.one / 2);
                listedRef[i] = q * listedRef[i];
                listedRef[i] += (Vector3.one / 2);
            }
            var result = ConvertVectorListToReference(listedRef);
            return result;
        }

        void ComputeAndAddRotatedReference(uint reference, uint newRef, Quaternion q)
        {
            March toCopy = GetMarch(reference);
            March newMarch = new March();
            newMarch.vertices = new List<Vector3>();

            for (int i = 0; i < toCopy.vertices.Count; ++i)
            {
                Vector3 v = toCopy.vertices[i];
                v -= Vector3.one / 2;
                v = q * v;
                v += Vector3.one / 2;
                v = new Vector3(Convert.ToInt32(v.x * 2), Convert.ToInt32(v.y * 2), Convert.ToInt32(v.z * 2));
                v /= 2; // looks useless but actually refines the values to show cleaner 
                newMarch.vertices.Add(v);
            }
            newMarch.tris = toCopy.tris;
            newMarch.reference = "";
            for (int i = 0; i < 8; ++i)
            {
                if (i == 4)
                    newMarch.reference = "-" + newMarch.reference;
                newMarch.reference = ((newRef & 1) == 1 ? "1" : "0") + newMarch.reference;
                newRef = newRef >> 1;
            }
            marches.Add(newMarch);
        }

        private void GenerateRotations()
        {
            for (uint i = 0; i < 256; ++i)
            {
                if (GetMarch(i) != null)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        uint refToRotate = Rotation(i, Quaternion.Euler(90 * x, 0, 0));
                        if (GetMarch(refToRotate) == null)
                        {
                            ComputeAndAddRotatedReference(i, refToRotate, Quaternion.Euler(90 * x, 0, 0));
                        }
                        refToRotate = Rotation(i, Quaternion.Euler(0, 90 * x, 0));
                        if (GetMarch(refToRotate) == null)
                        {
                            ComputeAndAddRotatedReference(i, refToRotate, Quaternion.Euler(0, 90 * x, 0));
                        }
                        refToRotate = Rotation(i, Quaternion.Euler(0, 0, x * 90));
                        if (GetMarch(refToRotate) == null)
                        {
                            ComputeAndAddRotatedReference(i, refToRotate, Quaternion.Euler(0, 0, 90 * x));
                        }
                    }
                }
            }
        }

        private void GenerateSharpVertices()
        {
            Marches = marches;
            if (GetMarch(0) == null)
            {
                Marches.Add(new March()
                {
                    reference = "0000-0000",
                    vertices = new List<Vector3>(),
                    tris = new List<int>()
                });
            }

            if (GetMarch(1) == null)
            {
                Marches.Add(new March()
                {
                    reference = "0000-0001",
                    vertices = new List<Vector3>()
                {
                    new Vector3(1,0.5f,1),
                    new Vector3(0.5f,1,1),
                    new Vector3(1,1,0.5f)
                },
                    tris = new List<int>() { 0, 1, 2 }
                });
            }

            if (GetMarch(3) == null)
            {
                Marches.Add(new March()
                {
                    reference = "0000-0011",
                    vertices = new List<Vector3>()
                {
                    new Vector3(0,0.5f,1),
                    new Vector3(1,0.5f,1),
                    new Vector3(1,1,0.5f),
                    new Vector3(0,1,0.5f),
                    new Vector3(0,0.5f,1),
                    new Vector3(1,1,0.5f),
                },
                    tris = new List<int>() { 0, 2, 1, 4, 3, 5 }
                });
            }
            if (GetMarch(6) == null)
            {
                Marches.Add(new March()
                {
                    reference = "0000-0110",
                    vertices = new List<Vector3>()
                {
                    new Vector3(1,1,0.5f),
                    new Vector3(1,0.5f,0),
                    new Vector3(0.5f,1,0),
                    new Vector3(0,1,0.5f),
                    new Vector3(0,0.5f,1),
                    new Vector3(0.5f,1,0),
                },
                    tris = new List<int>() { 0, 2, 1, 3, 5, 4 }
                });
            }
            if (GetMarch(7) == null)
            {
                Marches.Add(new March()
                {
                    reference = "0000-0111",
                    vertices = new List<Vector3>()
                {
                    new Vector3(0.5f,1,0),
                    new Vector3(0,1,0.5f),
                    new Vector3(0,0.5f,1),
                    new Vector3(1,0.5f,1),
                    new Vector3(1,0.5f,0),
                    new Vector3(1,0.5f,0),
                    new Vector3(1,0.5f,0),
                    new Vector3(0,0.5f,1),
                    new Vector3(0,1,0.5f),
                },
                    tris = new List<int>() { 0, 4, 1, 5, 3, 2, 6, 7, 8 }
                });
            }
            if (GetMarch(15) == null)
            {
                Marches.Add(new March()
                {
                    reference = "0000-1111",
                    vertices = new List<Vector3>()
                {
                    new Vector3(0,0.5f,0),
                    new Vector3(1,0.5f,0),
                    new Vector3(1,0.5f,1),
                    new Vector3(0,0.5f,1),
                    new Vector3(0,0.5f,0),
                    new Vector3(1,0.5f,1),
                },
                    tris = new List<int>() { 0, 1, 2, 4, 5, 3 }
                });
            }

            if (GetMarch(22) == null)
            {
                Marches.Add(new March()
                {
                    reference = "0001-0110",
                    vertices = new List<Vector3>()
                    {
                        new Vector3(0.5f,1,1),
                        new Vector3(0,1,0.5f),
                        new Vector3(0,0.5f,1),
                        new Vector3(0.5f,0,1),
                        new Vector3(1,0,0.5f),
                        new Vector3(1,0.5f,1),

                        new Vector3(0.5f,1,0),
                        new Vector3(1,1,0.5f),
                        new Vector3(1,0.5f,0),
                    },
                    tris = new List<int>() { 0,2,1,3,5,4,6,8,7 }
                });
            }

            if (GetMarch(24) == null)
            {
                Marches.Add(new March()
                {
                    reference = "0001-1000",
                    vertices = new List<Vector3>()
                    {
                        new Vector3(0.5f,1,0),
                        new Vector3(0,1,0.5f),
                        new Vector3(0,0.5f,0),
                        new Vector3(1,0,0.5f),
                        new Vector3(0.5f,0,1),
                        new Vector3(1,0.5f,1),
                    },
                    tris = new List<int>() { 0,1,2,3,4,5 }
                });
            }

            if (GetMarch(25) == null)
            {
                Marches.Add(new March()
                {
                    reference = "0001-1001",
                    vertices = new List<Vector3>()
                    {
                        new Vector3(1,0,0.5f),
                        new Vector3(1,1,0.5f),
                        new Vector3(0.5f,1,1),

                        new Vector3(0.5f,0,1),
                        new Vector3(0,1,0.5f),
                        new Vector3(0,0.5f,0),

                        new Vector3(0.5f,1,0),
                        new Vector3(1,0,0.5f),
                        new Vector3(0.5f,1,1),
                    },
                    tris = new List<int>() { 0,2,1,7,3,8,4,5,6 }
                });
            }

            if (GetMarch(27) == null)
            {
                Marches.Add(new March()
                {
                    reference = "0001-1011",
                    vertices = new List<Vector3>()
                    {
                        new Vector3(0,0,0.5f),
                        new Vector3(1,0.5f,0),
                        new Vector3(0,0.5f,0),

                        new Vector3(0,0,0.5f),
                        new Vector3(0.5f,1,1),
                        new Vector3(1,0.5f,0),

                        new Vector3(1,0.5f,0),
                        new Vector3(0.5f,1,1),
                        new Vector3(1,1,0.5f),

                        new Vector3(0,0,0.5f),
                        new Vector3(0.5f,0,1),
                        new Vector3(0.5f,1,1),                        
                    },
                    tris = new List<int>() { 0,1,2,4,5,6,7,8,9,10,11 }
                });
            }


            if (GetMarch(30) == null)
            {
                Marches.Add(new March()
                {
                    reference = "0001-1110",
                    vertices = new List<Vector3>()
                    {
                        new Vector3(0.5f,1,1),
                        new Vector3(1,1,0.5f),
                        new Vector3(1,0.5f,0),
                        new Vector3(0,0.5f,0),

                        new Vector3(0,0.5f,1),
                        new Vector3(1,0,0.5f),
                        new Vector3(0.5f,0,1),
                        new Vector3(1,0.5f,1),

                        new Vector3(0,0.5f,1),
                        new Vector3(0,0.5f,1),
                        new Vector3(1,1,0.5f),
                        new Vector3(1,0.5f,0),
                    },
                    tris = new List<int>() { 0,4,1,8,3,2,9,11,10,5,6,7 }
                });
            }

            if (GetMarch(60) == null)
            {
                Marches.Add(new March()
                {
                    reference = "0011-1100",
                    vertices = new List<Vector3>()
                    {
                        new Vector3(1,0.5f,1),
                        new Vector3(0,0.5f,1),
                        new Vector3(0,0,0.5f),
                        new Vector3(1,0,0.5f),

                        new Vector3(1,0.5f,0),
                        new Vector3(0,0.5f,0),
                        new Vector3(0,1,0.5f),
                        new Vector3(1,1,0.5f),

                        new Vector3(1,0.5f,1),
                        new Vector3(0,0,0.5f),
                        new Vector3(1,0.5f,0),
                        new Vector3(0,1,0.5f),
                    },
                    tris = new List<int>() { 0,2,1,8,3,9,4,6,5,10,7,11 }
                });
            }

            if (GetMarch(105) == null)
            {
                Marches.Add(new March()
                {
                    reference = "0110-1001",
                    vertices = new List<Vector3>()
                {
                    new Vector3(0.5f,0,1),
                    new Vector3(0,0,0.5f),
                    new Vector3(0,0.5f,1),
                    new Vector3(0.5f,1,1),

                    new Vector3(1,1,0.5f),
                    new Vector3(1,0.5f,1),
                    new Vector3(0.5f,0,0),
                    new Vector3(1,0,0.5f),

                    new Vector3(1,0.5f,0),
                    new Vector3(0.5f,1,0),
                    new Vector3(0,1,0.5f),
                    new Vector3(0,0.5f,0),
                },
                    tris = new List<int>() { 0,1,2,3,4,5,6,7,8,9,10,11 }
                });
            }


            if (GetMarch(113) == null)
            {
                Marches.Add(new March()
                {
                    reference = "0111-0001",
                    vertices = new List<Vector3>()
                {
                    new Vector3(0.5f,1,1),
                    new Vector3(1,1,0.5f),
                    new Vector3(1,0.5f,0),
                    new Vector3(0.5f,0,0),
                    new Vector3(0,0,0.5f),
                    new Vector3(0,0.5f,1),
                    new Vector3(0,0.5f,1),
                    new Vector3(1,1,0.5f),
                    new Vector3(1,1,0.5f),
                    new Vector3(0,0,0.5f),
                    new Vector3(1,0.5f,0),
                    new Vector3(0,0,0.5f),
                },
                    tris = new List<int>() { 0, 1, 5, 6, 7, 4, 8, 2, 9, 10, 3, 11 }
                });
            }

            if (GetMarch(177) == null)
            {
                Marches.Add(new March()
                {
                    reference = "1011-0001",
                    vertices = new List<Vector3>()
                    {
                        new Vector3(0.5f,1,1),
                        new Vector3(1,1,0.5f),
                        new Vector3(1,0,0.5f),
                        new Vector3(0.5f,0,0),
                        new Vector3(0,0.5f,0),
                        new Vector3(0,0.5f,1),

                        new Vector3(1,1,0.5f),
                        new Vector3(0.5f,0,0),
                        new Vector3(0,0.5f,1),
                        new Vector3(0.5f,0,0),
                        new Vector3(1,1,0.5f),
                        new Vector3(0,0.5f,1),
                    },
                    tris = new List<int>() { 1, 3, 5, 6, 2, 7, 8, 9, 4, 0, 10, 11 }
                });
            }

            



            marches = Marches;
            Debug.Log("refs = " + marches.Count());
        }

        private void CheckMissingMarches()
        {
            Debug.Log("Final Marching Cube Library Check");
            for (uint i = 0; i < 256; ++i)
            {
                if (GetMarch(i) == null)
                    Debug.Log("Missing March" + i.ToString());
            }
        }


        //Call only to fix corrupted Marching cubes
        private void CheckAndCleanLibrary()
        {
            GenerateSharpVertices();
            GenerateRotations();
            GenerateNegatives();
            marches = Marches;
            marches.Sort(new Comp());
            CheckMissingMarches();
            PrefabUtility.SaveAsPrefabAsset(gameObject, "Assets/marchingCubeLibrarySharp.prefab");
        }
    */
    #endregion
}
