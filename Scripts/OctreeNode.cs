using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctreeNode {
    public BoundingBox BBox;

    public OctreeNode _Parent;
    public OctreeNode Parent {
        set { Debug.Assert(_Parent == null, "Node already has a parent!"); _Parent = value; }
        get { return _Parent; }
    }

    private List<OctreeNode> Children;
    private List<GameObject> Objects;
    public List<GameObject> GetObjects() { return Objects; }
    //que for delisting game objects after colliding
    //Constructor
    public OctreeNode(Vec3 origin, Vec3 extents) {
        //create the bounding box for the node
        BBox = new BoundingBox(origin, extents);
        // will only allocate these things as they are required
        _Parent = null;
        Children = null;
        Objects = null;
    }

    public void MakeChildren() {
        Debug.Assert(Children == null, " Children already present on this OctreeNode");
        Children = new List<OctreeNode>();
        //Create the 8 child objects for this Octree Node
        //Calculate the bounds of the child objects (simplu myltiply by 0.5 in each dimension)
        Vec3 extents = BBox.Extents * 0.5f;
        //origin is in the center of the bounfing box offset is half new nounds in each dimentions
        Vec3 origin = BBox.Position;
        //A for loop is a bit wateful here
        Children.Add(new OctreeNode(new Vec3(origin.x - extents.x, origin.y - extents.y, origin.z - extents.z), extents));
        Children.Add(new OctreeNode(new Vec3(origin.x - extents.x, origin.y - extents.y, origin.z + extents.z), extents));
        Children.Add(new OctreeNode(new Vec3(origin.x - extents.x, origin.y + extents.y, origin.z - extents.z), extents));
        Children.Add(new OctreeNode(new Vec3(origin.x - extents.x, origin.y + extents.y, origin.z + extents.z), extents));
        Children.Add(new OctreeNode(new Vec3(origin.x + extents.x, origin.y - extents.y, origin.z - extents.z), extents));
        Children.Add(new OctreeNode(new Vec3(origin.x + extents.x, origin.y - extents.y, origin.z + extents.z), extents));
        Children.Add(new OctreeNode(new Vec3(origin.x + extents.x, origin.y + extents.y, origin.z - extents.z), extents));
        Children.Add(new OctreeNode(new Vec3(origin.x + extents.x, origin.y + extents.y, origin.z + extents.z), extents));

        foreach (OctreeNode child in Children) {
            child.Parent = this;
        }
    }

    void SendObjectsToChildren() {
        List<GameObject> delist = new List<GameObject>();
        //iterate through the objects in this list  and see which child they belong to 
        foreach (GameObject go in Objects) {
            //bounds for a game object can be obtained via the mesh/colider/renderer
            //https://docs.unity3d.com/SproteReference/Bounds.html sorce used for reference
            Bounds box = go.GetComponent<Renderer>().bounds;
            Vec3 goCentre = new Vec3(box.center);
            Vec3 goExtents = new Vec3(box.extents);
            foreach (OctreeNode child in Children) {
                if (child.BBox.ContainsObect(goCentre, goExtents)) {
                    if (child.AddObject(go)) {
                        //object successfuly added to child object
                        delist.Add(go);
                        break; //break out of this forloop back to go loop
                    }
                }
            }
        }
        foreach (GameObject go in delist) {
            Objects.Remove(go);
        }
    }

    public bool AddObject(GameObject a_object) {
        bool objectAdded = false;
        //If node has children attempt to add object to child of node
        if (Children != null && Children.Count > 0) {
            foreach (OctreeNode child in Children) {
                objectAdded = child.AddObject(a_object);
                if (objectAdded) {
                    break;
                }
            }
        }

        //if object not added then continue to attempt to add object to this node
        if (objectAdded != true) {

            if (Objects == null) {
                Objects = new List<GameObject>();
            }

            Bounds box = a_object.GetComponent<Renderer>().bounds;
            Vec3 goCentre = new Vec3(box.center);
            Vec3 goExtents = new Vec3(box.extents);
            if (BBox.ContainsObect(goCentre, goExtents)) {
                Objects.Add(a_object);
                objectAdded = true;
                //if our object count exceeds a certain amount then we initilaise children and redistribute objects
                if (Objects.Count >= 4 && Children == null) {
                    MakeChildren();
                    SendObjectsToChildren();
                }
            }
        }
        return objectAdded;
    }

    private bool RemoveObject(GameObject a_object, bool fromChildren = true) {
        if (Objects != null) {
            if (Objects.Remove(a_object)) {
                return true;
            }
        }
        if (fromChildren && Children != null) {
            foreach (OctreeNode child in Children) {
                if (child.RemoveObject(a_object)) {
                    return true;
                }
            }
        }
        return false;
    }
    public void Draw() {
        //draw the parent bound inbox
        BBox.Draw();
        if (Children != null) {
            //draw each child bounds
            foreach (OctreeNode child in Children) {
                child.Draw();
            }
        }
    }
    public void PerformCollisonTest() {

        //stepping through the children
        if (Children != null && Children.Count > 0) {
            foreach (OctreeNode child in Children) {
                if (Objects != null && Objects.Count > 1) {
                    List<GameObject> delist = new List<GameObject>();
                    List<GameObject> po = (Parent == null) ? null : Parent.GetObjects();

                    List<GameObject> collisionList = new List<GameObject>();
                    if (po != null) { collisionList.AddRange(po); }
                    collisionList.AddRange(Objects);
                    Debug.Log("Performing Collison Test");
                    for (int i = 0; i < collisionList.Count; ++i) {
                        //testing bounds against next object in list
                        Bounds ob1 = collisionList[i].GetComponent<Renderer>().bounds;
                        Vec3 ob1Centre = new Vec3(ob1.center);
                        Vec3 ob1Extents = new Vec3(ob1.extents);

                        for (int j = i + 1; j < collisionList.Count; ++j) {
                            if (collisionList[i] != collisionList[j]) {
                                Bounds ob2 = collisionList[j].GetComponent<Renderer>().bounds;
                                Vec3 ob2Center = new Vec3(ob2.center);
                                Vec3 ob2Extents = new Vec3(ob2.extents);
                                //testing two object for collison
                                if (ob1Centre.x - ob1Extents.x < ob2Center.x + ob2Extents.x && ob1Centre.x + ob1Extents.x > ob2Center.x - ob2Extents.x &&
                                    ob1Centre.y - ob1Extents.y < ob2Center.y + ob2Extents.y && ob1Centre.y + ob1Extents.y > ob2Center.y - ob2Extents.y &&
                                    ob1Centre.z - ob1Extents.z < ob2Center.z + ob2Extents.z && ob1Centre.z + ob1Extents.z > ob2Center.z - ob2Extents.z) {
                                    //object in collison with another object        
                                    delist.Add(collisionList[i]);
                                    delist.Add(collisionList[j]);
                                }
                            }
                        }
                    }
                    if (delist.Count > 0) {
                        foreach (GameObject go in delist) {
                            if (!Objects.Remove(go)) {
                                if (Parent != null) {
                                    Parent.RemoveObject(go, false);
                                }
                            }
                            GameObject.Destroy(go);
                        }
                    }
                }
            }
        }
    }
}