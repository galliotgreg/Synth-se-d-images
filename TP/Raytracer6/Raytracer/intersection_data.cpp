#include "stdafx.h"
#include "intersection_data.h"

intersection_data::intersection_data(vec3 position, vec3 normal, double t)
{
	this->position = position;
	this->normal = normal;
	this->t = t;
}

intersection_data::~intersection_data()
{
}

void intersection_data::setPosition(vec3 position)
{
	this->position = position;
}

void intersection_data::setNormal(vec3 normal)
{
	this->normal = normal;
}

void intersection_data::setT(double t)
{
	this->t = t;
}

vec3 intersection_data::getPosition()
{
	return this->position;
}

vec3 intersection_data::getNormal()
{
	return this->normal;
}

double intersection_data::getT()
{
	return this->t;
}
