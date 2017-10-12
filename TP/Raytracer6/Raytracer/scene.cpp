#include "stdafx.h"
#include "scene.h"
#include "utils.h"
#include <iostream>

scene::scene(){}

std::deque<sphere> scene::getObjets(){
	return this->objets;
}
void scene::ajouterObjet(sphere obj){
	this->objets.push_back( obj );
}

std::deque<lumiere> scene::getLumieres() {
	return this->lumieres;
}
void scene::ajouterLumiere(lumiere lum)
{
	this->lumieres.push_back(lum);
}

sphere * scene::intersection(ray rayon, intersection_data * intersectionData) {
	int found = -1;
	intersection_data intersection_aux(vec3(0,0,0),vec3(1,0,0),1);
	intersection_data intersection_minimun(vec3(0, 0, 0), vec3(1, 0, 0), 1);

	for (int i = 0; i < this->objets.size(); i++)
	{
		if (this->objets[i].intersection(rayon, &intersection_aux)) {
			if ( found == -1 || intersection_aux.getT() < intersection_minimun.getT() ) {
				found = i;
				intersection_minimun = intersection_aux;
			}
		}
	}
	if (found > -1) {
		*intersectionData = intersection_minimun;
		return &(this->objets[found]);
	}
	else
		return NULL;
}

// retour la couleur obtenu sur lintersection
vec3 scene::intensiteLumieres(sphere sphereOrigine, intersection_data intersectionCamera)
{
	bool couleurFound = false;
	vec3 couleurResultat = vec3(0,0,0);
	// pour chaque lumiere, lancer un rayon vers la sphere
	for each (lumiere l in this->getLumieres())
	{
		// rayon partant de la lumiere vers la sphere
		vec3 direction = (intersectionCamera.getPosition().soustraction(l.getCentre())).normalized();
		ray rayonLumiere = ray(l.getCentre(), direction);

		intersection_data intersection_lumiere(vec3(0, 0, 0), vec3(1, 0, 0), 1);
		sphere * sphere_resultat = this->intersection(rayonLumiere, &intersection_lumiere);

		// verifier si la sphere est la premiere a etre touche par le rayon
		//if( sphere_resultat != NULL && sphere_resultat->getIndex() == sphereOrigine.getIndex() ) {
		if (sphere_resultat != NULL
			&& equalsDouble(intersection_lumiere.getPosition().getX(),intersectionCamera.getPosition().getX())
			&& equalsDouble(intersection_lumiere.getPosition().getY(), intersectionCamera.getPosition().getY())
			&& equalsDouble(intersection_lumiere.getPosition().getZ(), intersectionCamera.getPosition().getZ())) {
			
			// couleur des lumieres
			// produitScalaire = < normalIntersection, -directionLumiere >
			// couleur = ColeurLumiere * IntensiteLumiere * produitscalaire.clamp(0,produitScalaire) // clamp evite valeurs negatives
			double prodScalaire = (intersectionCamera.getNormal()).produitScalaire((rayonLumiere.getDirection()).negative());
			vec3 couleurLumiere = l.getCouleur().multiplication(l.getIntensite()*clamp(prodScalaire,0,prodScalaire));

			// basee sur la distance
			couleurLumiere = vec3::intensiteDistance(couleurLumiere, intersection_lumiere.getT());
			if (!couleurFound) {
				couleurFound = true;
				couleurResultat = couleurLumiere;
			}
			else{
				couleurResultat = couleurResultat.addition( couleurLumiere );
			}
		}
	}
	return vec3( clampCouleur(couleurResultat) );
}

scene::~scene(){}