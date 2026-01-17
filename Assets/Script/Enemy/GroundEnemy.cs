using UnityEngine;



public class GroundEnemy : EnemyAI

{

    protected override void Update()

    {

        // Cập nhật trạng thái hiển thị lên Inspector

        currentState = state;



        base.Update();

        UpdateVisionConeColor();

    }



    private void UpdateVisionConeColor()

    {

        if (visionCone == null) return;

        bool shouldBeAlert = (state == State.Chase || state == State.Attacking || state == State.LostChase);

        visionCone.SetAlertMode(shouldBeAlert);

    }



    // Ghi đè logic LostChase để quái vẫn "biết" vị trí player trong một khoảng thời gian

    protected override void UpdateLostChase()

    {

        // Nếu Player hoàn toàn không tồn tại trong Scene thì quay về tuần tra

        if (GameObject.FindGameObjectWithTag("Player") == null)

        {

            state = State.Patrol;

            return;

        }



        Transform targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;



        if (chaseGraceTimer > 0f)

        {

            chaseGraceTimer -= Time.deltaTime;



            // Đuổi theo vị trí HIỆN TẠI của player dù không nhìn thấy

            Vector2 dir = (Vector2)targetPlayer.position - rb.position;



            // Kiểm tra khoảng cách tấn công ngay cả khi đang trong LostChase

            float dist = Vector2.Distance(rb.position, targetPlayer.position);

            if (dist <= attackDistance)

            {

                rb.linearVelocity = Vector2.zero;

                if (attackTimer <= 0f)

                {

                    state = State.Attacking;

                    attackTimer = attackCooldown;

                    animator.SetTrigger(hashAttack);

                }

                return;

            }



            // Di chuyển đuổi theo

            rb.linearVelocity = dir.normalized * GetCurrentSpeed(chaseSpeed);

            UpdateFacing(dir);

        }

        else

        {

            // Hết thời gian đuổi "mù", chuyển sang trạng thái đứng ngơ ngác hoặc tuần tra

            rb.linearVelocity = Vector2.zero;

            idleAfterLostTimer = idleAfterLostDuration;

            state = State.IdleAfterLost;

        }

    }

}