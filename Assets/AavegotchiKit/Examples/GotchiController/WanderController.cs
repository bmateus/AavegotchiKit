using UnityEngine;

namespace PortalDefender.AavegotchiKit.Examples
{
    // A simple controller that moves the gotchi around randomly

    public class WanderController : MonoBehaviour
    {
        Rigidbody2D rb2d_;

        Gotchi gotchi_;

        Vector2 currentVelocity_;
        Vector2 targetVelocity_;

        float decisionTime_;

        [SerializeField]
        public float speed_ = 10f;

        private void Awake()
        {
            rb2d_ = GetComponent<Rigidbody2D>();
            gotchi_ = GetComponent<Gotchi>();
        }

        private void Update()
        {
            if (decisionTime_ <= 0f)
            {
                decisionTime_ = Random.Range(0.5f, 2f);

                bool shouldMove = Random.Range(0f, 1f) > 0.5f;

                if (shouldMove)
                {
                    // decide on a random direction to move
                    float horizontal = Random.Range(-1f, 1f);
                    float vertical = Random.Range(-1f, 1f);

                    //save the movement vector
                    targetVelocity_ = new Vector3(horizontal * speed_, vertical * speed_);
                }
                else
                {
                    // just stand still
                    targetVelocity_ = Vector2.zero;
                }
            }
            else
            {
                decisionTime_ -= Time.deltaTime;
            }

        }

        private void FixedUpdate()
        {
            //icrease the current velocity towards the target velocity
            currentVelocity_ = Vector2.Lerp(currentVelocity_, targetVelocity_, Time.fixedDeltaTime * 4f);
            //apply the current velocity to the rigid body
            rb2d_.velocity = currentVelocity_;

            //set facing depending on movement direction using trigonometry
            if (targetVelocity_.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(targetVelocity_.y, targetVelocity_.x) * Mathf.Rad2Deg;
                if (angle > -45 && angle <= 45)
                    gotchi_.State.Facing = GotchiFacing.RIGHT;
                else if (angle > 45 && angle <= 135)
                    gotchi_.State.Facing = GotchiFacing.BACK;
                else if (angle > 135 || angle <= -135)
                    gotchi_.State.Facing = GotchiFacing.LEFT;
                else if (angle > -135 && angle <= -45)
                    gotchi_.State.Facing = GotchiFacing.FRONT;
            }

        }

    }
}