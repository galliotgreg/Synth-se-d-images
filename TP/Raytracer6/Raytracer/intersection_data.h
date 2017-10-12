#include "vec3.h"

#pragma once
class intersection_data
{
	vec3 position;
	vec3 normal;
	double t;

public:
	intersection_data(vec3 position, vec3 normal, double t);
	~intersection_data();

	void setPosition(vec3 position);
	void setNormal(vec3 normal);
	void setT(double t);

	vec3 getPosition();
	vec3 getNormal();
	double getT();
};

