using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

[CanEditMultipleObjects]
#endif
public class Bay : MonoBehaviour
{
    //public
    public Product product;

    public int maxProductsDepth;
    public int maxProductsWidth;
    public int maxProductsHeight;

    public struct ProductPlacement
    {
        public Vector3 position;
        public Quaternion rotation;
        public Product product;
        public bool active;

        public bool hasBeenSpawnedBefore;
    }

    public float GridPlacementVariance(float input)
    {
        return Random.Range(-input, input);
    }

    public float gridScale;

    //private
    private ProductPlacement[,,] productPool;

    private List<GameObject> combinedMeshes = new List<GameObject>();
    private Vector3 boundsOffset;
    private Vector3 boundsSize;
    private bool filling = false;
    private bool fill = false;
    private bool stopFill = false;

    /// <summary>
    /// Retruns true if currently filling
    /// </summary>
    public bool Filling { get { return filling; } }

    private void Start()
    {
        Init();
        UnloadAll();
    }

    public void Init()
    {
        Mesh productMesh = product.gameObject.GetComponentInChildren<MeshFilter>().sharedMesh;
        Vector3 productSize = productMesh.bounds.size;
        //Vector3 productCenter = new Vector3(
        //    (maxProductsWidth - 1) * gridScale / 2,
        //    productMesh.bounds.size.y * maxProductsHeight / 2,
        //    (maxProductsDepth - 1) * gridScale / 2);

        boundsOffset = Vector3.zero;
        boundsOffset.y = productMesh.bounds.size.y * maxProductsHeight / 2;

        boundsSize = new Vector3(
            productSize.x / 2 + maxProductsWidth * gridScale,
            productSize.y / 2 + maxProductsHeight * gridScale,
            productSize.z / 2 + maxProductsDepth * gridScale);

        GetComponent<BoxCollider>().center = boundsOffset;
        GetComponent<BoxCollider>().size = boundsSize;
    }

    public void AddItem(int index)
    {
    }

    public void RemoveItem(Transform user)
    {
        if (filling)
            return;
        if (productPool == null)
            return;
        float distanceFromUser = 1000;
        Vector3Int index = new Vector3Int();

        for (int y = 0; y < maxProductsHeight; y++)
        {
            for (int x = 0; x < maxProductsWidth; x++)
            {
                for (int z = 0; z < maxProductsDepth; z++)
                {
                    if (productPool[x, y, z].active && Vector3.Distance(user.position, productPool[x, y, z].position) < distanceFromUser)
                    {
                        distanceFromUser = Vector3.Distance(user.position, productPool[x, y, z].position);
                        index = new Vector3Int(x, y, z);
                    }
                }
            }
        }

        productPool[index.x, index.y, index.z].active = false;
        productPool[index.x, index.y, index.z].hasBeenSpawnedBefore = false;

        RebuildMesh();
    }

    public void RebuildMesh()
    {
        Fill(true);
    }

    public void UnloadGameObjects()
    {
        if (productPool != null)
            foreach (ProductPlacement o in productPool)
            {
                if (o.product != null && o.product.gameObject != null)
                    DestroyImmediate(o.product.gameObject);
            }
    }

    public void UnloadCombinedMeshes()
    {
        foreach (GameObject o in combinedMeshes)
        {
            DestroyImmediate(o);
        }
    }

    public void UnloadAll()
    {
        productPool = null;
        if (transform.childCount > 0)
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
    }

    public void Fill(bool activeOnly)
    {
        // UnloadAll();
        if (productPool == null)
            productPool = new ProductPlacement[maxProductsWidth, maxProductsHeight, maxProductsDepth];

        int counter = 0;
        for (int y = 0; y < maxProductsHeight; y++)
        {
            for (int x = 0; x < maxProductsWidth; x++)
            {
                for (int z = 0; z < maxProductsDepth; z++)
                {
                    //Dont create duplicates
                    if (productPool[x, y, z].active && productPool[x, y, z].product.gameObject)
                        continue;

                    if (activeOnly && productPool[x, y, z].active)
                        //Create the product
                        CreateProduct(x, y, z, productPool[x, y, z].position, productPool[x, y, z].rotation);
                    else if (!activeOnly)
                        CreateProduct(x, y, z, 0);
                }
            }
        }
        CombineMeshes();
        filling = false;
        fill = false;
    }

    public GameObject CreateGameObject(Vector3 position, Quaternion rotation)
    {
        GameObject gameObject = Instantiate(product.gameObject);
        gameObject.name = name;
        gameObject.transform.position = transform.position + transform.TransformDirection(position);
        gameObject.transform.rotation = rotation;
        gameObject.transform.SetParent(transform);

        return gameObject;
    }

    private void CreateProduct(int x, int y, int z, float variance)
    {
        Vector3 position = (new Vector3(x + (-maxProductsWidth / 2) + GridPlacementVariance(variance), 0, z + (-maxProductsDepth / 2) + GridPlacementVariance(variance)) * gridScale + //width and depth
            Vector3.up * y * product.gameObject.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.size.y);                                      //height

        Vector3 rotation = new Vector3(0, GridPlacementVariance(variance * 1000), 0); //rotation

        productPool[x, y, z].position = position;
        productPool[x, y, z].rotation = Quaternion.Euler(rotation);
        GameObject newProduct = CreateGameObject(position, Quaternion.Euler(rotation));

        //Optimize
        if (productPool[x, y, z].product == null)
            productPool[x, y, z].product = new Product();
        productPool[x, y, z].product.gameObject = newProduct;
        productPool[x, y, z].active = true;
    }

    private void CreateProduct(int x, int y, int z, Vector3 position, Quaternion rotation)

    {
        GameObject newProduct = CreateGameObject(position, rotation);

        //Optimize
        if (productPool[x, y, z].product == null)
            productPool[x, y, z].product = new Product();
        productPool[x, y, z].product.gameObject = newProduct;
        productPool[x, y, z].active = true;
    }

    public IEnumerator Fill(int amount, float speed, float variance)
    {
        if (filling)
            yield break; //probably put something in here, message box pop up or something
        else
            filling = true;

        if (productPool == null)
            productPool = new ProductPlacement[maxProductsWidth, maxProductsHeight, maxProductsDepth];

        int counter = 0;
        for (int y = 0; y < maxProductsHeight; y++)
        {
            for (int x = 0; x < maxProductsWidth; x++)
            {
                for (int z = 0; z < maxProductsDepth; z++)
                {
                    //Break the loop
                    if (amount <= 0 || stopFill == true)
                    {
                        stopFill = false;
                        fill = false;
                        filling = false;
                        yield break;
                    }

                    //Dont create duplicates
                    if (productPool[x, y, z].active && productPool[x, y, z].product.gameObject != null)
                        continue;

                    //Create the product
                    if (productPool[x, y, z].hasBeenSpawnedBefore)
                    {
                        CreateProduct(x, y, z, productPool[x, y, z].position, productPool[x, y, z].rotation);
                        amount--;
                        continue;
                    }
                    else
                        CreateProduct(x, y, z, variance);

                    productPool[x, y, z].hasBeenSpawnedBefore = true;
                    amount--;

                    //Delay time
                    yield return new WaitForSeconds(speed);
                }
            }
        }
        CombineMeshes();
        filling = false;
        fill = false;
    }

    public void CombineMeshes()
    {
        // combine meshes
        List<CombineInstance> combine = new List<CombineInstance>();
        List<Mesh> meshPool = new List<Mesh>();

        int i = 0;
        for (int y = 0; y < maxProductsHeight; y++)
        {
            for (int x = 0; x < maxProductsWidth; x++)
            {
                for (int z = 0; z < maxProductsDepth; z++)
                {
                    if (i >= 50)
                    {
                        Mesh newMesh = new Mesh();
                        newMesh.CombineMeshes(combine.ToArray(), true);
                        meshPool.Add(newMesh);
                        combine = new List<CombineInstance>();
                        i = 0;
                    }

                    if (productPool[x, y, z].product.gameObject == null)
                        continue;

                    MeshFilter meshFilter = productPool[x, y, z].product.gameObject.GetComponentInChildren<MeshFilter>();
                    CombineInstance c = new CombineInstance();
                    c.mesh = meshFilter.sharedMesh;
                    c.transform = meshFilter.transform.localToWorldMatrix;

                    combine.Add(c);
                    i++;
                }
            }
        }
        UnloadGameObjects();
        UnloadCombinedMeshes();
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine.ToArray(), true);
        meshPool.Add(combinedMesh);

        //UnloadGameObjects();

        i = 0;
        foreach (Mesh m in meshPool)
        {
            GameObject newObject = new GameObject("(Combined Mesh)" + product.name + i.ToString());
            newObject.transform.SetParent(transform);
            newObject.AddComponent<MeshFilter>();
            newObject.AddComponent<MeshRenderer>();
            newObject.GetComponent<MeshFilter>().sharedMesh = m;
            newObject.GetComponent<MeshRenderer>().sharedMaterial = product.gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial;
            combinedMeshes.Add(newObject);
            i++;
        }
    }

    /*
    public static void DrawCube(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Matrix4x4 cubeTransform = Matrix4x4.TRS(position, rotation, scale);
        Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

        Gizmos.matrix *= cubeTransform;

        Gizmos.DrawCube(Vector3.zero, Vector3.one);

        Gizmos.matrix = oldGizmosMatrix;
    }

    private void OnDrawGizmos()
    {
        //Fuck this math shit
        for (int x = 0; x < maxProductsWidth; x++)
        {
            for (int z = 0; z < maxProductsDepth; z++)
            {
                Vector3 origin = transform.position + new Vector3(x, 0, z) * gridScale;
                Gizmos.color = new Color(1, 0, 1);
                Gizmos.DrawWireMesh(product.gameObject.GetComponentInChildren<MeshFilter>().sharedMesh, origin);
            }
        }

        Gizmos.color = new Color(0, 0, 1);

        DrawCube(boundsOffset, transform.parent.rotation, boundsSize);
    }
    */
}