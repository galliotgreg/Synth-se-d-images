#include "stdafx.h"
#include "ray.h"

ray::ray(vec3 origine, vec3 direction) {
	this->set(origine, direction);
}

void ray::set(vec3 origine, vec3 direction) {
	this->setOrigine(origine);
	this->setDirection(direction);
}
void ray::setOrigine(vec3 origine) {
	this->origine = origine;
}
void ray::setDirection(vec3 direction) {
	this->direction = direction;
}
vec3 ray::getOrigine() {
	return this->origine;
}
vec3 ray::getDirection(){
	return this->direction;
}

vec3 ray::position(double t){
	// r = x0 + t*u
	return this->origine.addition( this->direction.multiplication( t ) );
}

ray::~ray(){}