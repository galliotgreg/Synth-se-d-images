#include "vec3.h"
#include "sphere.h"

#pragma once
class lumiere// : sphere
{
public:
	vec3 centre;
	vec3 couleur;
	double intensite;	// valeur de intensite de la lumiere : [0,1]
	//int index;

public:
	lumiere(vec3 centre, vec3 couleur, double intensite);

	void set(vec3 centre, vec3 couleur, double intensite);
	void setCentre(vec3 centre);
	void setCouleur(vec3 couleur);
	void setIntensite(double intensite);
	vec3 getCentre();
	vec3 getCouleur();
	double getIntensite();

	~lumiere();
};

