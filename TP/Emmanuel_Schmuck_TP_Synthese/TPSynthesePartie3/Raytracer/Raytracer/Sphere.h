#pragma once


#include "vec.h"


class sphere {

	vec3 center;
	float radius;
	vec3 color;

public:

	// constructors
	sphere() { center = vec3(0,0,0); radius = 1; color = vec3(0,0,0); };
	sphere(vec3 c, float r, vec3 col) { center = c; radius = r; color = col; };

	// get, set
	vec3 getCenter() { return center; };
	float getRadius() { return radius; };
	vec3 getColor() { return color; };
	void setCenter(vec3 c) { center = c; };
	void setRadius(float r) { radius = r; };
	void setColor(vec3 col) { color = col; };
	void set(vec3 c, float r) { center = c; radius = r; };

	// intersect between a ray and this sphere
	bool intersect(ray r, float &t) {
		bool result = false;

		float delta, a, b, c, t1, t2;

		a = 1.0;
		b = 2.0 * (r.getDirection().dot(r.getOrigin()-center));
		c = (r.getOrigin()-center).normSquared() - radius*radius;

		delta = b*b - 4.0 * a*c;

		t1 = (-b + sqrt(delta)) / (2 * a);
		t2 = (-b - sqrt(delta)) / (2 * a);

		//cout << t1 << " " << t2 << endl;

		if (delta >= 0 && (t1>0 || t2>0)) {
			result = true;
			if (t2 > 0) { t = t2; }
			else if (t1 > 0) { t = t1; }
		
		}

		return result;
	
	}



};