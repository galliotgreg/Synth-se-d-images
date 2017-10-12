#pragma once

#include <cmath>
using namespace std;

class vec3 {

	float x;
	float y;
	float z;

public:

	// constructors
	vec3() { x = 0; y = 0; z = 0; };
	vec3(float _x, float _y, float _z) { x = _x; y = _y; z = _z; };

	// get, set
	float getX() { return x; };
	float getY() { return y; };
	float getZ() { return z; };
	void setX(float f) { x = f; };
	void setY(float f) { y = f; };
	void setZ(float f) { z = f; };
	void set(float _x, float _y, float _z) { x = _x; y = _y; z = _z; };

	// vector assign, add, sub, multiply, divide (member to member)
	void operator = (vec3 v) { x = v.getX(); y = v.getY(); z = v.getZ(); };
	vec3 operator + (vec3 v) { vec3 u(x + v.getX(), y + v.getY(), z + v.getZ()); return u; };
	vec3 operator - (vec3 v) { vec3 u(x - v.getX(), y - v.getY(), z - v.getZ()); return u; };
	vec3 operator * (vec3 v) { vec3 u(x * v.getX(), y * v.getY(), z * v.getZ()); return u; };
	vec3 operator / (vec3 v) { vec3 u(x / v.getX(), y / v.getY(), z / v.getZ()); return u; };

	// scalar multiply and divide
	vec3 operator * (float f) { vec3 u(x*f, y*f, z*f); return u; };
	vec3 operator / (float f) { vec3 u(x / f, y / f, z / f); return u; };

	// norm and norm squared
	float norm() { return sqrt(x*x + y*y + z*z); };
	float normSquared() { return x*x + y*y + z*z; };

	// normalize the vector and returns a clone
	vec3 normalize() { float n = norm(); vec3 u(x / n, y / n, z / n); return u; };

	// scalar product (dot product)
	float dot(vec3 v) { return (x*v.getX() + y*v.getY() + z*v.getZ()); };

	// vector product (cross product)
	vec3 cross(vec3 v) { vec3 c(y*v.getZ() - z*v.getY(), z*v.getX() - x*v.getZ(), x*v.getY() - y*v.getX()); return c; };

	// vector lerp
	static vec3 lerp(vec3 v1, vec3 v2, float alpha) {
		vec3 v;
		v.setX((1 - alpha)*v1.getX() + alpha*v2.getX());
		v.setY((1 - alpha)*v1.getY() + alpha*v2.getY());
		v.setZ((1 - alpha)*v1.getZ() + alpha*v2.getZ());
		return v; };
};