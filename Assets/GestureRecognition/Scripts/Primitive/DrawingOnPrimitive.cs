using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using gestureUtil;
using gesture;

namespace primitive
{
    /// <summary>
    /// Drawing On Primitive
    /// - saves the points drawn on the primitive
    /// - when the wand lifts off the primitive,
    ///   the painting is converted to a gesture and matched
    /// </summary>
    [RequireComponent(typeof(MagicWand))]
    public class DrawingOnPrimitive : MonoBehaviour
    {
        [SerializeField]
        private int m_iNumberOfPoints = 1024;
        [SerializeField]
        private float m_fMinimumDistance = 0.3f;

        private MagicWand m_magicWand;
        private Transform m_transform;
        private Transform m_transBeginning;
        private Transform m_transEnd;
        private FixedSizeQueue<Vector3> points;
        private Vector3 m_v3LastPoint;

        private GestureConverter m_converter;
        private GestureMatcher m_matcher;

        private ParticleSystem m_psDrawing;

        private SpellType[] m_gestureLUT = new SpellType[]
        {
            SpellType.FIREBALL, SpellType.FIREBALL,
            SpellType.METEOR, SpellType.METEOR,
            SpellType.SHIELD, SpellType.SHIELD,
            SpellType.MAGICMISSILE, SpellType.MAGICMISSILE,
            SpellType.LASERBEAM, SpellType.LASERBEAM
        };

        void Awake()
        {
            m_transform = transform;

            m_transBeginning = m_transform.Find("beginningPoint");
            Assert.IsNotNull(m_transBeginning);

            m_transEnd = m_transform.Find("endPoint");
            Assert.IsNotNull(m_transEnd);

            points = new FixedSizeQueue<Vector3>(m_iNumberOfPoints);

            m_v3LastPoint = Vector3.zero;

            m_converter = GameObject.FindGameObjectWithTag("GestureObject").GetComponent<GestureConverter>();
            Assert.IsNotNull(m_converter);

            m_matcher = m_converter.GetComponent<GestureMatcher>();
            Assert.IsNotNull(m_matcher);

            m_magicWand = GetComponent<MagicWand>();
            Assert.IsNotNull(m_magicWand);

            m_psDrawing = GetComponentInChildren<ParticleSystem>();
        }

        void OnTriggerEnter(Collider c)
        {
            if (c.CompareTag("Primitive"))
                m_psDrawing.Play();

            OnTriggerStay(c);
        }

        /// <summary>
        /// "draws" points on the primitive
        /// </summary>
        /// <param name="c">Plane Collider</param>
        void OnTriggerStay(Collider c)
        {
            // only for primitives
            if (!c.CompareTag("Primitive"))
                return;

            // calculate the hitting point
            RaycastHit hit;
            Vector3 wand = m_transEnd.position - m_transBeginning.position;
            if (c.Raycast(new Ray(m_transBeginning.position, (m_transEnd.position - m_transBeginning.position)),
                out hit,
                wand.magnitude))
            {
                m_psDrawing.transform.position = hit.point;

                if (Vector3.Distance(hit.point, m_v3LastPoint) > m_fMinimumDistance)
                {
                    // add to the debug queue
                    points.Enqueue(hit.point);

                    LineRenderer line = c.gameObject.GetComponent<LineRenderer>();
                    if (line != null)
                    {
                        line.positionCount = points.Count;
                        line.SetPositions(points.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Converts the 3D points to 2D (on the plane)
        /// Creates a gesture and matches the gesture
        /// </summary>
        /// <param name="c"></param>
        void OnTriggerExit(Collider c)
        {
            // only for primitives
            if (!c.CompareTag("Primitive"))
                return;

            // convert 3D points to 2D points on the plane
            Vector3[] rawPoints = points.ToArray();
            Vector2[] points2D = new Vector2[rawPoints.Length];
            Matrix4x4 matrix = c.transform.worldToLocalMatrix;
            for (int i = 0; i < rawPoints.Length; ++i)
            {
                Vector3 localPoint = matrix * rawPoints[i];
                points2D[i] = new Vector2(localPoint.x, localPoint.z);
            }

            // convert 2D points to a gesture
            GestureObject g = m_converter.CreateGestureFrom2DData(ref points2D);

            // match the gesture
            gestureTypes type;
            bool valid = m_matcher.Match(g, out type);
            
            // charge wand if valid
            if (valid)
            {
                m_magicWand.LoadWand(m_gestureLUT[(int)type]);
                StartCoroutine(SuckInParticles());
            }
            m_psDrawing.Stop();

            // reduce one try
            Primitive primitive = c.GetComponent<Primitive>();
            if (primitive == null) Debug.LogError("This primitive needs a primitive script!");
            else
            {
                primitive.reduceTry(valid);
            }

            // clear the lines
            points.Clear();
            LineRenderer line = c.gameObject.GetComponent<LineRenderer>();
            if (line != null)
            {
                line.positionCount = 0;
            }
        }

        IEnumerator SuckInParticles()
        {
            m_psDrawing.GetComponent<particleAttractorMove>().active = true ;
            while (m_psDrawing.IsAlive())
                yield return null;
            m_psDrawing.GetComponent<particleAttractorMove>().active = false;

            yield return null;
        }
    }
}
