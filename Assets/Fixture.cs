using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixture : MonoBehaviour
{
    public Bay bay;
    public bool removeItem = false;
    public bool fill = false;
    public bool stopFill = false;
    public GameObject[,] productPool;

    private bool filling = false;

    /// <summary>
    /// Retruns true if currently filling
    /// </summary>
    public bool Filling { get { return filling; } }

    private void Update()
    {
        if (removeItem)
            RemoveItem(transform);

        if (fill)
            Fill();
    }

    private void Start()
    {
        // do not modify, fuck this math shit
        GetComponent<BoxCollider>().center = bay.product.gameObject.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.center + (Vector3.right * (bay.maxProductsWidth / 2) * bay.gridScale) + (Vector3.forward * (bay.maxProductsDepth / 2) * bay.gridScale);
        GetComponent<BoxCollider>().size = bay.product.gameObject.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.size + new Vector3(bay.maxProductsWidth, 0, bay.maxProductsDepth) * bay.gridScale;
        StartCoroutine(Fill());
    }

    public void AddItem(int index)
    {
    }

    public void RemoveItem(Transform user)
    {
        removeItem = false;
        float distanceFromUser = 1000;
        GameObject toRemove = null;
        foreach (GameObject o in productPool)
        {
            if (o == null)
                continue;
            else if (Vector3.Distance(user.position, o.transform.position) < distanceFromUser)
            {
                distanceFromUser = Vector3.Distance(user.position, o.transform.position);
                toRemove = o;
            }
        }

        if (toRemove)
            Destroy(toRemove);
    }

    public IEnumerator Fill()
    {
        if (filling)
            yield break; //probably put something in here, message box pop up or something
        else
            filling = true;

        if (productPool == null)
            productPool = new GameObject[bay.maxProductsWidth, bay.maxProductsDepth];

        int counter = 0;
        for (int x = 0; x < bay.maxProductsWidth; x++)
        {
            for (int z = 0; z < bay.maxProductsDepth; z++)
            {
                if (stopFill == true)
                {
                    stopFill = false;
                    fill = false;
                    filling = false;
                    yield break;
                }

                if (productPool[x, z] != null)
                    continue;

                yield return new WaitForSeconds(0.5f);
                GameObject newProduct = Instantiate(bay.product.gameObject, transform);
                newProduct.name = counter++.ToString() + bay.product.name;
                newProduct.transform.localPosition = new Vector3(x, 0, z) * bay.gridScale;
                productPool[x, z] = newProduct;
            }
        }
        filling = false;
        fill = false;
    }

    private void OnDrawGizmos()
    {
        //Fuck this math shit
        for (int x = 0; x < bay.maxProductsWidth; x++)
        {
            for (int z = 0; z < bay.maxProductsDepth; z++)
            {
                Vector3 origin = transform.position + new Vector3(x, 0, z) * bay.gridScale;
                Gizmos.color = new Color(1, 0, 1);
                Gizmos.DrawWireMesh(bay.product.gameObject.GetComponentInChildren<MeshFilter>().sharedMesh, origin);
            }
        }

        Vector3 boundsSize = bay.product.gameObject.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.size + Vector3.left * bay.maxProductsWidth + Vector3.forward * bay.maxProductsDepth;
        Gizmos.DrawWireCube(transform.position + (Vector3.forward * (bay.maxProductsDepth / 2 + 0.1f) * bay.gridScale) + (Vector3.right * (bay.maxProductsWidth / 2 + 0.1f) * bay.gridScale), boundsSize * bay.gridScale);
    }
}