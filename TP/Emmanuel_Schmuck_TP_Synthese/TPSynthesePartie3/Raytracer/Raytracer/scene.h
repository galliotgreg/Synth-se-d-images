#pragma once

#include "sphere.h"
#include <iostream>

class scene {

	vec3 cameraPosition;
	sphere* spheres;
	int n;
	int n_max;


public :

	//constructors
	scene() { };
	scene(vec3 campos, int nmax) { n_max = nmax; spheres = new sphere[n_max]; n = 0; cameraPosition = campos; };

	//get, set
	vec3 getCameraPosition() { return cameraPosition; };
	void setCameraPosition(vec3 pos) { cameraPosition = pos; };

	//add a sphere to the scene
	void addSphere(vec3 pos, float radius, vec3 color) {
		if (n == n_max) { return; }
		sphere s(pos,radius,color);
		spheres[n] = s;
		n++;
	};

	//intersect between a ray and all spheres
	bool intersect(ray r, float &t, vec3 &rgb) {
		t = 999999;
		bool result = false;
		float t_temp=0;

		for (int i = 0; i < n; i++) {

			if (spheres[i].intersect(r, t_temp)) {
				if (t_temp < t) {
					t = t_temp;
					rgb = spheres[i].getColor();
					result = true;
				}
			}
		}

		return result;

	}
	
















};
