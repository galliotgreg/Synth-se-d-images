#include "vec3.h"
#include "ray.h"
#include "intersection_data.h"

#pragma once
class sphere
{
public:
	typedef enum propriete {
		diffuse,
		speculaire,
		transparente
	};

private:
	double rayon;
	vec3 centre;
	vec3 albedo;	// couleur entre [0,1]
	propriete proprieteSphere;
	double intensite;	// valeur de intensite referent a chaque propriete du materiel : [0,1]

public:
	sphere(double rayon, vec3 centre, vec3 albedo, propriete proprieteSphere, double intensite);
	sphere(double rayon, vec3 albedo, propriete proprieteSphere);		// centre en (0,0,0); intensite = 1;

	void set(double rayon, vec3 centre, vec3 albedo, propriete proprieteSphere, double intensite);
	void setRayon(double rayon);
	void setCentre(vec3 centre);
	void setAlbedo(vec3 albedo);
	void setPropriete(propriete proprieteSphere);
	void setIntensite(double intensite);

	double getRayon();
	vec3 getCentre();
	vec3 getAlbedo();
	propriete getPropriete();
	double getIntensite();

	bool isMirroir();
	bool isTransparente();

	ray rebondir( ray rayon, intersection_data intersection );
	ray * refraction( ray rayon, intersection_data intersection, bool entre );

	bool intersection( ray rayon, intersection_data * intersectionData );

	~sphere();
};
