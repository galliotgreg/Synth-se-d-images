#include "stdafx.h"
#include "sphere.h"
#include <math.h>
#include <algorithm>
#include <iostream>
using namespace std;

sphere::sphere(double rayon, vec3 centre, vec3 albedo, sphere::propriete proprieteSphere, double intensite) {
	this->set(rayon, centre, albedo, proprieteSphere, intensite);
}
sphere::sphere(double rayon, vec3 albedo, sphere::propriete proprieteSphere) {		// centre en (0,0,0); intensite = 1;
	this->set(rayon, vec3(0, 0, 0), albedo, proprieteSphere, 1. );
}

void sphere::set(double rayon, vec3 centre, vec3 albedo, sphere::propriete proprieteSphere, double intensite) {
	this->setRayon(rayon);
	this->setCentre(centre);
	this->setAlbedo(albedo);
	this->setPropriete(proprieteSphere);
	this->setIntensite(intensite);
}
void sphere::setRayon(double rayon) {
	this->rayon = rayon;
}
void sphere::setAlbedo(vec3 albedo) {
	this->albedo = albedo;
}
void sphere::setCentre(vec3 centre) {
	this->centre = centre;
}
void sphere::setPropriete(sphere::propriete proprieteSphere) {
	this->proprieteSphere = proprieteSphere;
}
void sphere::setIntensite(double intensite) {
	this->intensite = intensite;
}

double sphere::getRayon() {
	return this->rayon;
}
vec3 sphere::getCentre() {
	return this->centre;
}
vec3 sphere::getAlbedo() {
	return this->albedo;
}
sphere::propriete sphere::getPropriete() {
	return this->proprieteSphere;
}
double sphere::getIntensite() {
	return this->intensite;
}

bool sphere::isMirroir(){
	//return this->intensite > .999;
	return this->proprieteSphere == sphere::propriete::speculaire;
}
bool sphere::isTransparente() {
	//return this->intensite > .999;
	return this->proprieteSphere == sphere::propriete::transparente;
}

ray sphere::rebondir(ray rayon, intersection_data intersection) {
	// vecteur I
	vec3 vecI = rayon.getDirection().normalized();

	// calculer la normal
	vec3 normale = (intersection.getPosition().soustraction(this->getCentre())).normalized();
	vec3 normale_p = normale.multiplication(vecI.produitScalaire(normale));

	// decalage de l'intersection vers la normale
	intersection.setPosition( intersection.getPosition().addition(normale.multiplication(.001)) );

	// creer le rayon
	return ray(intersection.getPosition(), (normale_p.multiplication(-2.)).addition(vecI));
}

// retour le rayon de sortie de l'sphere : NULL si il ny a pas de refraction
ray * sphere::refraction(ray rayon, intersection_data intersection, bool entre )
{
	// on considere que le indice de refraction de l'exterieur = 1
	double indiceRefraction;
	if (entre) {
		indiceRefraction = (1. / this->intensite);
	}
	else {
		indiceRefraction = this->intensite;
	}

	// tr = (ind1 / ind2)*(i-scal(i,n)*n)
	double scalaire = (rayon.getDirection()).produitScalaire(intersection.getNormal());
	vec3 tR = (rayon.getDirection()).soustraction( (intersection.getNormal()).multiplication( scalaire ));

	double radical = 1.-(pow( indiceRefraction,2 )*pow( (1.-scalaire),2));

	if (radical > 0) {
		// tn = -n*sqrt[1-pow(ind1 / ind2)*pow(1-scal(i,n))]
		vec3 tN = ((intersection.getNormal()).negative()).multiplication(sqrt(radical));

		vec3 t = (tR.addition(tN)).normalized();

		// decalage de l'intersection vers la normale (dans la sphere)
		intersection.setPosition( (intersection.getPosition()).addition(tN.multiplication(0.01)));

		// trouver intersection dans la sphere
		intersection_data intersectionInterne = intersection_data(vec3(0, 0, 0), vec3(0, 0, 0), 0);
		this->intersection( ray(intersection.getPosition(), t), &intersectionInterne);

		// inversion de la normale
		intersectionInterne.setNormal((intersectionInterne.getNormal()).negative());
		// decalage de l'intersection vers la normale (dans la sphere)
		intersectionInterne.setPosition((intersectionInterne.getPosition()).addition((intersectionInterne.getNormal()).multiplication(0.01)));

		return new ray( intersectionInterne.getPosition(), t );
	}
	else {
		return NULL;
	}
}

// retour si il y de intersection
bool sphere::intersection(ray rayon, intersection_data * intersectionData) {
	vec3 unitaire = rayon.getDirection().normalized();
	double scalaire = unitaire.produitScalaire( rayon.getOrigine().soustraction( this->getCentre() ) );
	double normeCarre = rayon.getOrigine().soustraction(this->getCentre()).normeCarre();
	double rayonCarre = this->getRayon()*this->getRayon();

	// ax2 + bx + c = 0
	double a = 1.;
	double b = scalaire * 2.;
	double c = normeCarre - rayonCarre;
	// Solutions
	double found = false;
	double solution;

	double racineInterieur = ((b*b) - (4. * a*c));

	if (racineInterieur < 0) {
		return false;
	}
	else
	{
		double racine = sqrt( racineInterieur );
		double t1 = (b*(-1.) + racine) / (2.*a);
		double t2 = (b*(-1.) - racine) / (2.*a);

		if (t2 > 0) {
			found = true;
			solution = t2;
		}
		else {
			if (t1 > 0) {
				found = true;
				solution = t1;
			}
			else {
				return false;
			}
		}
	}
	if (found) {
		intersectionData->setPosition(rayon.position(solution));
		intersectionData->setNormal( (rayon.position(solution).soustraction(this->getCentre())).normalized() );
		intersectionData->setT(solution);
	}
	return found;
}

sphere::~sphere(){}