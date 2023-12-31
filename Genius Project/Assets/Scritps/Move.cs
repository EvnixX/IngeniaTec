using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private Rigidbody2D rg2D;

    [Header("Movimiento")]
    private float movimientoHorizontal = 0f;
    [SerializeField] private float velocidadMovimiento;
    [Range(0, 0.3f)] [SerializeField] private float suavizadoMovimiento;
    private Vector3 velocidad = Vector3.zero;
    private bool mirarDerecha = true;

    [Header("Salto")]
    [SerializeField] private float fuerzaSalto;
    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector3 dimensionesCaja;
    [SerializeField] private bool enSuelo;

    public AudioClip audioFX;

    internal bool Salto = false;

    public ControlType controlType;
    internal float joystickValue;


    private void Start()
    {
        rg2D = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (controlType == ControlType.keyboard)
        {
            movimientoHorizontal = Input.GetAxisRaw("Horizontal") * velocidadMovimiento;
        }
        else
        {
            movimientoHorizontal = joystickValue * velocidadMovimiento;
        }

        if (Input.GetButtonDown("Jump"))
        {         
            Salto = true;
        }
    }
    private void FixedUpdate()
    {
        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0f, queEsSuelo);

        Mover(movimientoHorizontal * Time.fixedDeltaTime, Salto);

        Salto = false;
    }
    private void Mover(float mover, bool saltar)
    {
        Vector3 velocidadObjetivo = new Vector2(mover, rg2D.velocity.y);
        rg2D.velocity = Vector3.SmoothDamp(rg2D.velocity, velocidadObjetivo, ref velocidad, suavizadoMovimiento);

        if (mover > 0 && !mirarDerecha)
        {
            Girar();
        }
        else if (mover < 0 && mirarDerecha)
        {
            Girar();
        }
        if (enSuelo && saltar)
        {
            enSuelo = false;
            rg2D.AddForce(new Vector2(0f, fuerzaSalto));
            AudioSource.PlayClipAtPoint(audioFX, gameObject.transform.position);
        }
    }
    private void Girar()
    {
        mirarDerecha = !mirarDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCaja);
    }

    public enum ControlType
    {
        keyboard,
        Touch
    }
}


