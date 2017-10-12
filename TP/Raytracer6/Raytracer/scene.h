#include "sphere.h"
#include "lumiere.h"
#include "intersection_data.h"
#include <deque>

#pragma once
class scene
{
	std::deque<sphere> objets;
	std::deque<lumiere> lumieres;

public:
	scene();

	std::deque<sphere> getObjets();
	void ajouterObjet(sphere obj);

	std::deque<lumiere> getLumieres();
	void ajouterLumiere(lumiere lum);

	sphere * intersection(ray rayon, intersection_data * intersectionData);
	// retour la couleur obtenu sur lintersection
	vec3 intensiteLumieres(sphere sphereOrigine, intersection_data intersectionCamera);

	~scene();
};

