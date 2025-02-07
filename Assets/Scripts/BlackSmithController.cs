using UnityEngine;

namespace Com.ZiomtechStudios.BlacksmithingVR.Player
{
    public class BlackSmithController : MonoBehaviour
    {
        #region "Private Serialized FIelds"
        
        [SerializeField][Tooltip("Sorting reference to camera that is players vision ")] private Camera playerCam;
        [SerializeField][Tooltip("Minimum distance we want the player to be before they can interact with a particular object.")] private float minDist;
        [SerializeField][Tooltip("Reference to the object that player is holding.")] private GameObject holdingObj;
        [Tooltip("When the ray casted from camera hits an object we gain some info about the object.")] private RaycastHit hit;
        [SerializeField] [Tooltip("The calculated distance between the player and an object at the center of their line ofm sight.")] private float dist;
        
        public MetalInfoController metalInfoContr;

        //location we want the metal bars to be when it is held by the tongs
        public Transform holdingPos;

        //Rigidbody of obj being held
        private Rigidbody objRB;

        //script attached to the metal bar we are holding
        private MetalBarController mBContr;

        //reference to the script attacehd to the cooler
        public QuenchingController quenchCont;

        //bool to tell if we are carrying an obj at the moment
        private bool isCarrying;
        #endregion

        //This function makes sure that the object we are holding, maintains the same position and rotation relatiev to the position of the blacksmiths tool
        //fun the update the users UI so that they are given stats about the metal bar they are looking at and close enough too
        public void updateMetalInfo()
        {
            metalInfoContr.metalTemp = decimal.Round((decimal)mBContr.MetalBarStruct.metalTemp, 2).ToString();
            metalInfoContr.metalType = mBContr.MetalBarStruct.metalType;
        }

        // Start is called before the first frame update
        void Start()
        {
            isCarrying = false;
            GameStats.ambientTemp = 22.0f;
        }

        // Update is called once per frame
        void Update()
        {
            //Reference the ray that is once per frame is being fired through the camera
            Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            //Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);
            //Did the ray hit anything? if so store it in hit
            if (Physics.Raycast(ray, out hit))
            {
                //Objects in our line of sight, but are we close enough to it?
                dist = Vector3.Distance(transform.position, hit.transform.position);
            }

            //picking up
            if (!isCarrying)
            {
                holdingObj = hit.collider.gameObject;
                objRB = holdingObj.GetComponent<Rigidbody>();
                mBContr = holdingObj.GetComponent<MetalBarController>();
                metalInfoContr = holdingObj.transform.Find("UI Canvas/Text (TMP)").gameObject.GetComponent<MetalInfoController>();
                updateMetalInfo();
                if (Input.GetButtonDown("Interact") && dist <= minDist)
                {
                    isCarrying = true;
                    if (mBContr.InWater)
                        quenchCont.IsQuenching = false;
                    mBContr.ThermalTimer = 0.0f;
                }
            }
            //dropping
            else if (isCarrying && Input.GetButtonDown("Interact"))
            {
                isCarrying = false;
                objRB.constraints = RigidbodyConstraints.None;
                metalInfoContr.Reset();
                mBContr.ThermalTimer = 0.0f;
                Physics.IgnoreLayerCollision(gameObject.layer, holdingObj.layer, false);
                Vector3 ogScale = holdingObj.transform.localScale;
                holdingObj.transform.SetParent(null, true);
                holdingObj.transform.localScale = ogScale;

            }
            //carrying obj
            else if (isCarrying)
            {
                updateMetalInfo();
            }
        }
    }
}
