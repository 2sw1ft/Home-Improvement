﻿using UnityEngine;

using ModComponentMapper;

namespace HomeImprovement
{
    internal class PaperClutter : GameObjectSearchFilter
    {
        private static PaperClutter instance;

        private PaperClutter()
        {
        }

        public static PaperClutter FilterInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PaperClutter();
                }
                return instance;
            }
        }

        public override SearchResult Filter(GameObject gameObject)
        {
            if (!gameObject.activeInHierarchy || gameObject.layer == vp_Layer.Gear)
            {
                return SearchResult.SKIP_CHILDREN;
            }

            if (gameObject.name.StartsWith("OBJ_PaperDebris") || gameObject.name.StartsWith("OBJ_PaperDeco") || gameObject.name.StartsWith("OBJ_WallDecoPatterned") || gameObject.name.StartsWith("OBJ_CalendarDeco"))
            {
                return SearchResult.INCLUDE_SKIP_CHILDREN;
            }

            qd_Decal decal = gameObject.GetComponent<qd_Decal>();
            if (decal)
            {
                if (decal.texture.name.StartsWith("FX_DebrisPaper") || decal.texture.name.StartsWith("FX_DebrisMail"))
                {
                    return SearchResult.INCLUDE_SKIP_CHILDREN;
                }
            }


            return SearchResult.CONTINUE;
        }

        internal static void Prepare(GameObject gameObject)
        {
            Renderer renderer = Utils.GetLargestBoundsRenderer(gameObject);
            if (renderer == null)
            {
                return;
            }

            if (gameObject.name.StartsWith("Decal-"))
            {
                gameObject.transform.localRotation = Quaternion.identity;
            }

            Collider collider = gameObject.GetComponentInChildren<Collider>();
            if (collider != null)
            {
                gameObject = collider.gameObject;
            }

            GameObject collisionObject = new GameObject();
            collisionObject.name = "PaperDecalRemover-" + gameObject.name;
            collisionObject.transform.parent = gameObject.transform.parent;
            collisionObject.transform.position = gameObject.transform.position;

            gameObject.transform.parent = collisionObject.transform;

            gameObject = collisionObject;

            if (collider == null)
            {
                Bounds bounds = renderer.bounds;

                BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.size = bounds.size;
                boxCollider.center = bounds.center - gameObject.transform.position;
            }

            BreakDown breakDown = gameObject.AddComponent<BreakDown>();
            breakDown.m_YieldObject = new GameObject[] { Resources.Load("GEAR_CrumpledPaper") as GameObject };
            breakDown.m_YieldObjectUnits = new int[] { 1 };
            breakDown.m_TimeCostHours = 1f / 60;
            breakDown.m_BreakDownAudio = "PLAY_HARVESTINGPAPER";
            breakDown.m_LocalizedDisplayName = new LocalizedString() { m_LocalizationID = "GAMEPLAY_Paper" };
            breakDown.m_UsableTools = new GameObject[0];

            ChangeLayer changeLayer = gameObject.AddComponent<ChangeLayer>();
            changeLayer.Layer = vp_Layer.InteractivePropNoCollideGear;
            changeLayer.Recursively = true;
        }
    }
}