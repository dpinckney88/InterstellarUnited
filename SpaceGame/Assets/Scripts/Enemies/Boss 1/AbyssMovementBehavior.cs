using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class AbyssMovementBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    private IEnemyBasic behavior;
    private Vector3[] bounds;
    private bool collidedWithPlayer = false;
    private bool skip = false;
    Coroutine currentMovementCoroutine;
    Coroutine MainMovementOperator;
    void Start()
    {
        behavior = gameObject.GetComponent<IEnemyBasic>();
        bounds = Utilities.GetScreenBounds();
        MainMovementOperator = StartCoroutine(Movement());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Movement()
    {
        while (true)
        {
            int randomMovement = Random.Range(0, 3);
            IEnumerator selectedMovement;
            switch (randomMovement)
            {
                case 0:
                    selectedMovement = DiagonalCO();
                    break;
                case 1:
                    selectedMovement = StrafeCO();
                    break;
                case 2:
                    selectedMovement = WanderingCO();
                    break;
                default:
                    selectedMovement = WanderingCO();
                    break;
            }
            InitateMovement(selectedMovement);
            yield return new WaitForSeconds(10);
        }
    }

    IEnumerator DiagonalCO()
    {
        Vector3[][] diagonalPaths = new Vector3[4][];

        //Get screenbounds
        Vector3[] bounds = Utilities.GetScreenBounds();
        Vector3 minPoint = bounds[0];
        Vector3 maxPoint = bounds[1];

        //Get half the width and height of the sprite and use as the offset.
        float xOffset = gameObject.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        float yOffset = gameObject.GetComponent<SpriteRenderer>().bounds.size.y / 2;

        //Set the points the sprite will move between
        Vector3 destinationTopRight = maxPoint;
        Vector3 destinatioTopLeft = new Vector3(minPoint.x, maxPoint.y, 0);
        Vector3 destinationBottomLeft = minPoint;
        Vector3 destinationBottomRight = new Vector3(maxPoint.x, minPoint.y, 0);

        //Ignore camera offset
        destinationTopRight.z = 0;
        destinatioTopLeft.z = 0;
        destinationBottomLeft.z = 0;
        destinationBottomRight.z = 0;

        //Set the paths in the array
        diagonalPaths[0] = new Vector3[] { destinationTopRight, destinationBottomLeft };
        diagonalPaths[1] = new Vector3[] { destinatioTopLeft, destinationBottomRight };
        diagonalPaths[2] = new Vector3[] { destinationBottomLeft, destinationTopRight };
        diagonalPaths[3] = new Vector3[] { destinationBottomRight, destinatioTopLeft };

        Vector3 destination;
        Vector3 start;
        int randomPath = Random.Range(0, 4);

        while (true)
        {
            destination = diagonalPaths[randomPath][1];
            start = diagonalPaths[randomPath][0];
            skip = false;

            //Do not let the sprite move past the min and max points.
            destination.x = Mathf.Clamp(destination.x, minPoint.x + xOffset, maxPoint.x - xOffset);
            destination.y = Mathf.Clamp(destination.y, minPoint.y + yOffset, maxPoint.y - yOffset);
            start.x = Mathf.Clamp(start.x, minPoint.x + xOffset, maxPoint.x - xOffset);
            start.y = Mathf.Clamp(start.y, minPoint.y + yOffset, maxPoint.y - yOffset);

            //Instantly move to the start point.
            gameObject.transform.position = start;
            yield return null;
            while (gameObject.transform.position != destination && !skip)
            {
                if (collidedWithPlayer)
                {
                    skip = true;
                }
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, destination, 10 * Time.deltaTime);

                yield return null;
            }
            yield return new WaitForSeconds(.5f);
            randomPath = Random.Range(0, 4);
        }
    }

    IEnumerator StrafeCO()
    {
        bool shouldMoveRight = true;
        //Get screenbounds
        Vector3[] bounds = Utilities.GetScreenBounds();
        Vector3 minPoint = bounds[0];
        Vector3 maxPoint = bounds[1];

        //Get half the width and height of the sprite and use as the offset.
        float xOffset = gameObject.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        float yOffset = gameObject.GetComponent<SpriteRenderer>().bounds.size.y / 2;

        //Set the points the sprite will move between
        Vector3 destinationRight = maxPoint;
        Vector3 destinationLeft = new Vector3(minPoint.x, maxPoint.y, 0);
        //Ignore camera offset
        destinationRight.z = 0;
        destinationLeft.z = 0;

        //Do not let the sprite move past the min and max points.
        destinationRight.x = Mathf.Clamp(destinationRight.x, minPoint.x + xOffset, maxPoint.x - xOffset);
        destinationRight.y = Mathf.Clamp(destinationRight.y, minPoint.y + yOffset, maxPoint.y - yOffset);

        destinationLeft.x = Mathf.Clamp(destinationLeft.x, minPoint.x + xOffset, maxPoint.x - xOffset);
        destinationLeft.y = Mathf.Clamp(destinationLeft.y, minPoint.y + yOffset, maxPoint.y - yOffset);


        //move to the top right corner
        while (true)
        {
            if (shouldMoveRight && gameObject.transform.position != destinationRight)
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, destinationRight, 5 * Time.deltaTime);
            }
            else if (!shouldMoveRight && gameObject.transform.position != destinationLeft)
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, destinationLeft, 5 * Time.deltaTime);
            }
            else
            {
                shouldMoveRight = !shouldMoveRight;
            }
            yield return null;
            //If we collide with the player, go the other direction
            if (collidedWithPlayer)
            {
                shouldMoveRight = !shouldMoveRight;
            }
        }
    }

    IEnumerator WanderingCO()
    {
        System.Random r = new System.Random();
        //Randomly choose a distance in X and Y direction
        int randomX = r.Next(0, 5);
        int randomY = r.Next(0, 5);

        //Randomly decide if the value is negative or positve;
        randomX *= r.Next(0, 2) * 2 - 1;
        randomY *= r.Next(0, 2) * 2 - 1;

        //Set destination vector
        Vector3 destination = new Vector3(randomX, randomY, 0) + transform.position;

        Vector3 minPoint = bounds[0];
        Vector3 maxPoint = bounds[1];

        //Get half the width and height of the sprite and use as the offset.
        float xOffset = gameObject.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        float yOffset = gameObject.GetComponent<SpriteRenderer>().bounds.size.y / 2;

        //Do not let the sprite move base the min and max points.
        destination.x = Mathf.Clamp(destination.x, minPoint.x + xOffset, maxPoint.x - xOffset);
        destination.y = Mathf.Clamp(destination.y, minPoint.y + yOffset, maxPoint.y - yOffset);

        while (destination != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, 2 * Time.deltaTime);
            yield return null;
        }
    }
    private void InitateMovement(IEnumerator movementPattern)
    {
        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
        }
        currentMovementCoroutine = StartCoroutine(movementPattern);
    }

    private void StopMovement()
    {
        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.layer == 8)
        {
            collidedWithPlayer = true;
            //Check to see if we change movement
            int flip = Random.Range(0, 2);
            if (flip == 1)
            {
                StopCoroutine(MainMovementOperator);
                MainMovementOperator = StartCoroutine(Movement());
            }
            //deal damage
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            collidedWithPlayer = false;
        }
    }
}
