// Raytracer.cpp : Définit le point d'entrée pour l'application console.
//

#include "stdafx.h"
#include <iostream>
#include "vec3.h"
#include "ray.h"
#include "sphere.h"
#include "lumiere.h"
#include "scene.h"
#include "utils.h"
using namespace std;

#define PI 3.14159265

/* fonction printVec
	imprimer les valeurs des coordonnees du vecteur. Utilise le "iostream" por l'impression sur console

	Params => a : vecteur a etre imprime
	Retour => void : le resultat est imprime sur console
*/
void printVec( vec3 a ) {
	cout << a.toString() << endl;
}

/* fonction obtenirDirectionRayon
	creer le vecteur de direction pour le pixel indique (j,i)

	Params => i : ligne du tableau - ecran
	Params => j : column du tableau - ecran
	Params => h : hauteur du tableau - ecran
	Params => w : longueur du tableau - ecran
	Params => fov : 'field of view' de la camera
	Retour => vec3 : la direction du rayon partant de la camera
*/
vec3 obtenirDirectionRayon( int i, int j, int h, int w, int fov ) {
	return (vec3((double)(j - w / 2), (double)(i - h / 2), (double)((-1)*(w / (2 * std::tan(((fov / 2) * PI / 180.0))))))).normalized();
}

vec3 obtenirCouleur( ray rayon, scene sceneObjets, int mirroir ){

	intersection_data intersection_scene(vec3(0, 0, 0), vec3(1, 0, 0), 1);
	sphere * sphere_resultat = sceneObjets.intersection(rayon,&intersection_scene);

	if ( sphere_resultat != NULL ) {
		vec3 albedo;
	
		// speculaire
		if ( sphere_resultat->isMirroir() && mirroir < 3 ) {
			ray rayon_rebondi = sphere_resultat->rebondir( rayon, intersection_scene );
			albedo = obtenirCouleur(rayon_rebondi, sceneObjets, mirroir + 1);

			// albedo_reflex = intensite * reflexion + (1-intensite) * albedo_objet
			vec3 albedo_reflex = albedo.multiplication(sphere_resultat->getIntensite());
			vec3 albedo_objet = (sphere_resultat->getAlbedo()).multiplication((1-sphere_resultat->getIntensite()));
			albedo = albedo_reflex.addition(albedo_objet);
		}
		// transparente
		else if (sphere_resultat->isTransparente()) {
			ray * rayon_refraction = sphere_resultat->refraction(rayon, intersection_scene, true );
			if (rayon_refraction != NULL) {
				albedo = obtenirCouleur(*rayon_refraction, sceneObjets, mirroir+1);
			}
			else {
				cout << "saiu"<< endl;
				albedo = vec3(0, 0, 0);
			}
		}
		// diffuse
		else {
			albedo = sphere_resultat->getAlbedo();
		}
		// couleur de la lumiere : [0,1]
		vec3 couleurLumiere = (sceneObjets.intensiteLumieres(*sphere_resultat, intersection_scene)).division(255);

		// ajouter lumiere ambiance 30%
		couleurLumiere = clamp(couleurLumiere.addition((vec3(1,1,1).multiplication(.3))), 0, 1);

		// resultat entre la couleur de lobjet et lumiere = couleur[0,1] * lumiere
		//couleur = couleur.addition(couleurLumiere).division(2);
		albedo = albedo.multiplication(couleurLumiere);

		return albedo;
	}
	else {
		return vec3(0., 0., 0.);
	}
}

int main()
{
	/*


	TP01 - Partie 04


	*/

	// --------------------------------------------------------------
	// --------------------------------------------------------------
	// Test :: Mirroir
	//*
	vec3 origine = vec3(0, 0, 0);

	scene scene1 = scene();
	// speculaires
	scene1.ajouterObjet( sphere(10, vec3(0, 0, -50), couleurToAlbedo(vec3(255, 0, 0)), sphere::propriete::speculaire, 1));		// sphere mirroir
	// transparents
	scene1.ajouterObjet( sphere(5,	vec3(5, 10, -30), couleurToAlbedo(vec3(255, 0, 0)), sphere::propriete::transparente, 1.3));		// sphere transparent
	// objets
	scene1.ajouterObjet(sphere(30, vec3(-40, -40, -60), couleurToAlbedo(vec3(100, 200, 0)), sphere::propriete::diffuse, 1));		// sphere jaune - chaise
	scene1.ajouterObjet(sphere(2, vec3(0, 15, -40), couleurToAlbedo(vec3(255, 0, 0)), sphere::propriete::diffuse, 1));				// sphere petite - rouge	bizarre
	// murs
	scene1.ajouterObjet( sphere(2000, vec3(0, -2000-50, 0), couleurToAlbedo(vec3(150, 50, 0)), sphere::propriete::diffuse, 1));	// sphere marron - sol
	scene1.ajouterObjet( sphere(2000, vec3(0, 2000 + 50, 0), couleurToAlbedo(vec3(50, 25, 0)), sphere::propriete::diffuse, 1));	// sphere noir - plafond
	scene1.ajouterObjet( sphere(2000, vec3(-2000-50, 0, 0), couleurToAlbedo(vec3(96, 96, 96)), sphere::propriete::diffuse, 1));	// sphere gris - gauche
	scene1.ajouterObjet( sphere(2000, vec3(2000+50, 0, 0), couleurToAlbedo(vec3(96, 96, 96)), sphere::propriete::diffuse, 1));	// sphere gris - droit
	scene1.ajouterObjet( sphere(2000, vec3(0, 0, -2000-100), couleurToAlbedo(vec3(125,125,125)), sphere::propriete::diffuse, 1));	// sphere gris - fond
	scene1.ajouterObjet( sphere(2000, vec3(0, 0, 2000+10), couleurToAlbedo(vec3(255, 255, 255)), sphere::propriete::diffuse, 1));	// sphere gris - arriere
	// lumieres
	scene1.ajouterLumiere( lumiere( vec3(0, 40, -40), vec3(255, 255, 255), 1) );		// lumiere jaune

	// Tableau
	int w = 512;
	int h = 512;
//	w = 64;
//	h = 64;
	int fov = 60;

	// initialisation
	pixel ** colors = new pixel*[h];
	for (int i = 0; i<h; i++)
		colors[i] = new pixel[w];

	// Execute les rayons
	bool resultat = false;
	double t = 0.;
	for (int i = 0; i < h; i++)
		for (int j = 0; j < w; j++) {
			vec3 direction = obtenirDirectionRayon(i, j, h, w, fov);
			ray rayCamera = ray(origine, direction);
			
			vec3 couleur = obtenirCouleur( rayCamera, scene1, 0 );

			// definir la couleur
			colors[i][j].set(
				albedoToCouleur(couleur).getX(),
				albedoToCouleur(couleur).getY(),
				albedoToCouleur(couleur).getZ()
			);
		}

	// sauvegarde dans le fichier "sphere.ppm"
	save_img("sphereMirroir", w, h, colors);
	//*/

	// arreter le logiciel, afin de regarder les resultats. sinon, il se fermera automatiquement
	/*
	int x;
	cin >> x;
	/*/

    return 0;
}

