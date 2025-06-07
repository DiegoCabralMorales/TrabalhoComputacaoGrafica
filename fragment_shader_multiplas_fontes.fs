// parametros da iluminacao ambiente e difusa
uniform vec3 lightPos1; // define coordenadas de posicao da luz #1
uniform vec3 lightPos2; // define coordenadas de posicao da luz #2
uniform vec3 lightPos3; // define coordenadas de posicao da luz #3
uniform vec3 lightPos4; // define coordenadas de posicao da luz #4

uniform float ka; // coeficiente de reflexao ambiente
uniform float kd; // coeficiente de reflexao difusa

// parametros da iluminacao especular
uniform vec3 viewPos; // define coordenadas com a posicao da camera/observador
uniform float ks; // coeficiente de reflexao especular
uniform float ns; // expoente de reflexao especular

// parametro com a cor da(s) fonte(s) de iluminacao
uniform vec3 lightColor1;
uniform vec3 lightColor2;
uniform vec3 lightColor3;
uniform vec3 lightColor4;

// parametros recebidos do vertex shader
varying vec2 out_texture; // recebido do vertex shader
varying vec3 out_normal; // recebido do vertex shader
varying vec3 out_fragPos; // recebido do vertex shader
uniform sampler2D samplerTexture;

//Função para Calcular a Iluminação de uma Única Luz
vec3 calculateLight(vec3 lightPos, vec3 lightColor, int direcao) {

    vec3 norm = normalize(out_normal);
    vec3 lightDir = direcao * normalize(lightPos - out_fragPos);
    float diff = max(dot(norm, lightDir), 0.0);

    float distance = length(lightPos - out_fragPos);
    float attenuation = 1.0 / (1.0 + 0.09 * distance + 0.032 * distance * distance);
    vec3 diffuse = kd * diff * lightColor * attenuation;

    vec3 viewDir = normalize(viewPos - out_fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), ns);
    vec3 specular = ks * spec * lightColor;

    return diffuse + specular;
}

void main(){
    // Calcula a reflexão ambiente, que é constante para todas as luzes.
    vec3 ambient = ka * vec3(1.0, 1.0, 1.0);

    vec3 lightContribution1 = calculateLight(lightPos1, lightColor1, -1);
    vec3 lightContribution2 = calculateLight(lightPos2, lightColor2, 1);
    vec3 lightContribution3 = calculateLight(lightPos3, lightColor3, 1);
    vec3 lightContribution4 = calculateLight(lightPos4, lightColor4, 1);


    // Soma todas as contribuições de luz (ambiente, difusa e especular de cada luz).
    vec3 totalLighting = ambient + lightContribution1 + lightContribution2 + lightContribution3 + lightContribution4;

    // Aplica a iluminação à textura do objeto.
    vec4 textureColor = texture2D(samplerTexture, out_texture);
    vec4 result = vec4(totalLighting, 1.0) * textureColor;

    // Define a cor final do fragmento.
    gl_FragColor = result;
}