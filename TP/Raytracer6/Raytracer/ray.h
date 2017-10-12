#include "vec3.h"

#pragma once
class ray
{
	vec3 origine;
	vec3 direction;

public:
	ray(vec3 origine, vec3 direction);

	void set(vec3 origine, vec3 direction);
	void setOrigine(vec3 origine);
	void setDirection(vec3 direction);
	vec3 getOrigine();
	vec3 getDirection();

	vec3 position( double t );

	~ray();
};

