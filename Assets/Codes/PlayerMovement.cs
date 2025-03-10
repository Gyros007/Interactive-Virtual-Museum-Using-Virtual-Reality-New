﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIDE_Data;


public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
	
	public float speed = 12f;
	public float gravity = -9.81f;
	public float jumpHeight = 3f;
	
	public Transform groundCheck;
	public float groundDistance = 0.4f;
	public LayerMask groundMask;
	
	Vector3 velocity;
	bool isGrounded;

	// Update is called once per frame
	void Update()
	{
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

		if (isGrounded && velocity.y < 0)
		{
			velocity.y = 0f;
		}

		if (!isGrounded)
		{
			velocity.y += gravity * Time.deltaTime;
		}

		controller.Move(velocity * Time.deltaTime);


		if (!VD.isActive)
		{
			float x = Input.GetAxis("Horizontal");
			float z = Input.GetAxis("Vertical");

			Vector3 move = transform.right * x + transform.forward * z;

			controller.Move(move * speed * Time.deltaTime);

			if (Input.GetButtonDown("Jump") && isGrounded)
			{
				velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
			}

			if (Input.GetButtonDown("Shift"))
			{
				speed = 24f;
			}
			else if (Input.GetButtonUp("Shift"))
			{
				speed = 12f;
			}
		}
	}
}
