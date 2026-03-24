using mset;
using UnityEngine;

namespace CompositeBuildables
{
    public class LightingRefreshHelper : MonoBehaviour
    {
        private Renderer[] _ownRenderers;
        private MaterialPropertyBlock _readBlock;
        private MaterialPropertyBlock _writeBlock;

        private BaseCell _baseCell;
        private SubRoot _subRoot;
        private Renderer _referenceRenderer;

        private void Awake()
        {
            _readBlock = new MaterialPropertyBlock();
            _writeBlock = new MaterialPropertyBlock();
            CacheContext();
        }

        private void OnEnable()
        {
            CacheContext();
            _referenceRenderer = null;
        }

        private void LateUpdate()
        {
            CacheContext();

            if (_ownRenderers == null || _ownRenderers.Length == 0)
                return;

            if (_subRoot == null || _subRoot.interiorSky == null)
                return;

            if (_referenceRenderer == null || !IsRendererValid(_referenceRenderer))
                _referenceRenderer = FindReferenceRenderer();

            float powerLoss = 0f;

            if (_referenceRenderer != null)
            {
                _readBlock.Clear();
                _referenceRenderer.GetPropertyBlock(_readBlock);
                powerLoss = _readBlock.GetFloat(ShaderPropertyID._UwePowerLoss);
            }

            ApplyLighting(powerLoss);
        }

        private void CacheContext()
        {
            _ownRenderers = GetComponentsInChildren<Renderer>(true);
            _baseCell = GetComponentInParent<BaseCell>();
            _subRoot = GetComponentInParent<SubRoot>();
        }

        private Renderer FindReferenceRenderer()
        {
            if (_baseCell == null)
                return null;

            Renderer[] roomRenderers = _baseCell.GetComponentsInChildren<Renderer>(true);
            if (roomRenderers == null || roomRenderers.Length == 0)
                return null;

            for (int i = 0; i < roomRenderers.Length; i++)
            {
                Renderer candidate = roomRenderers[i];
                if (!IsRendererValid(candidate))
                    continue;

                if (BelongsToThisObject(candidate))
                    continue;

                return candidate;
            }

            return null;
        }

        private bool BelongsToThisObject(Renderer renderer)
        {
            if (renderer == null || _ownRenderers == null)
                return false;

            for (int i = 0; i < _ownRenderers.Length; i++)
            {
                if (_ownRenderers[i] == renderer)
                    return true;
            }

            return false;
        }

        private bool IsRendererValid(Renderer renderer)
        {
            if (renderer == null)
                return false;

            Material[] mats = renderer.sharedMaterials;
            if (mats == null || mats.Length == 0)
                return false;

            for (int i = 0; i < mats.Length; i++)
            {
                Material mat = mats[i];
                if (mat != null && mat.HasProperty(ShaderPropertyID._UwePowerLoss))
                    return true;
            }

            return false;
        }

        private void ApplyLighting(float powerLoss)
        {
            Sky sky = _subRoot.interiorSky;

            for (int i = 0; i < _ownRenderers.Length; i++)
            {
                Renderer renderer = _ownRenderers[i];
                if (renderer == null)
                    continue;

                sky.ApplyFast(renderer, 0);

                _writeBlock.Clear();
                renderer.GetPropertyBlock(_writeBlock);
                _writeBlock.SetFloat(ShaderPropertyID._UwePowerLoss, powerLoss);
                renderer.SetPropertyBlock(_writeBlock);
            }
        }
    }
}