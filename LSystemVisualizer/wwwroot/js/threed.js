import * as THREE from "three";

let scene;
let camera;
let renderer;

let cube;

export function initialize() {
    // Select the canvas from the document
    var canvReference = document.getElementById("my_canvas");

    scene = new THREE.Scene();
    camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
    renderer = new THREE.WebGLRenderer({
        canvas: canvReference
    });
    console.log(renderer);
    renderer.setSize(window.innerWidth, window.innerHeight);
    document.body.appendChild(renderer.domElement);

    const geometry = new THREE.BoxGeometry( 1, 1, 1 );
    const material = new THREE.MeshBasicMaterial( { color: 0x00ff00 } );
    cube = new THREE.Mesh( geometry, material );
    console.log(cube);
    scene.add( cube );

    camera.position.z = 5;
    renderer.setAnimationLoop( animate );
}



function animate() {
    renderer.render( scene, camera );
}


window.initialize = initialize;