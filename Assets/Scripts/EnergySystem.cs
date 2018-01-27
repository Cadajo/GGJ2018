﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySystem : MonoBehaviour {

    public Thruster thrusterTopLeft;
    public Thruster thrusterTopRight;
    public Thruster thrusterBottomLeft;
    public Thruster thrusterBottomRight;
    List<Thruster> thrusters;

    float energyInc = 0.1f;
    float energyTank = 4;
    new Rigidbody rigidbody;
	public float floatFactor = 30.0f;
    public float AccelerationFactor = 5.0f;
    
    void Start() {
        rigidbody = GetComponent<Rigidbody>();
        thrusters = new List<Thruster>();
        thrusters.Add(thrusterTopLeft);
        thrusters.Add(thrusterBottomLeft);
        thrusters.Add(thrusterBottomRight);
        thrusters.Add(thrusterTopRight);
    }

    void Update () {
        // Substract
        if (Input.GetKeyDown("a")) {
            energyTank += thrusterTopLeft.SubtractEnergy(energyInc);
        }
        if (Input.GetKeyDown("s")) {
            energyTank += thrusterBottomLeft.SubtractEnergy(energyInc);
        }

        if (Input.GetKeyDown("d")) {
            energyTank += thrusterBottomRight.SubtractEnergy(energyInc);
        }
        if (Input.GetKeyDown("f")) {
            energyTank += thrusterTopRight.SubtractEnergy(energyInc);
        }

        // Add
        if (Input.GetKeyDown("q")
            && energyTank >= energyInc) {
            energyTank -= thrusterTopLeft.AddEnergy(energyInc);
        }
        if (Input.GetKeyDown("w")
            && energyTank >= energyInc) {
            energyTank -= thrusterBottomLeft.AddEnergy(energyInc);
        }

        if (Input.GetKeyDown("e")
            && energyTank >= energyInc) {
            energyTank -= thrusterBottomRight.AddEnergy(energyInc);
        }
        if (Input.GetKeyDown("r")
            && energyTank >= energyInc) {
            energyTank -= thrusterTopRight.AddEnergy(energyInc);
        }
    }

	void FixedUpdate() {
		/* TODO: make tendency to unflip. this doesn't work
		rigidbody.rotation = Quaternion.Euler(rigidbody.rotation.x * 0.99f, rigidbody.rotation.y, rigidbody.rotation.z * 0.99f);
		*/

		// float
		rigidbody.AddForce(transform.up * floatFactor * Mathf.Max(1 - transform.position.y, 0));

		thrusters.ForEach((Thruster thruster) => {
			// overcharge causes upwards bump
			float overcharge = thruster.GetOvercharge();
			float energy = thruster.GetEnergy();
			if (overcharge > 1) {
				rigidbody.AddForceAtPosition(transform.up * overcharge * AccelerationFactor / 10, thruster.transform.position);

			}

			// forward thrust
			rigidbody.AddForceAtPosition(transform.forward * energy * overcharge * AccelerationFactor, thruster.transform.position);
		});
	}

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contactPoint in collision.contacts)
        {
            Thruster.DamageType type;
            if (collision.gameObject.tag == "Beam") // TODO: Create this tag
            {
                type = Thruster.DamageType.Beam;
            }
            else // Any other type will be trated as a wall
            {
                type = Thruster.DamageType.Wall;
            }

            contactPoint.thisCollider.gameObject.GetComponent<Thruster>()
                .TakeDamage(rigidbody.velocity.magnitude, type);
        }
    }

	void OnGUI () {
		GUI.Label(new Rect(10, 10, 100, 20),
		"TL: " + thrusterTopLeft.GetEnergy().ToString()
        + "/" + thrusterTopLeft.GetHP().ToString());
		GUI.Label(new Rect(10, 20, 100, 20), 
		"BL: " + thrusterBottomLeft.GetEnergy().ToString()
        + "/" + thrusterBottomLeft.GetHP().ToString());
		GUI.Label(new Rect(10, 30, 100, 20), 
		"BR: " + thrusterBottomRight.GetEnergy().ToString()
        + "/" + thrusterBottomRight.GetEnergy().ToString());
		GUI.Label(new Rect(10, 40, 100, 20), 
		"TR: " + thrusterTopRight.GetEnergy().ToString()
        + "/" + thrusterTopRight.GetHP().ToString());
	}
}
