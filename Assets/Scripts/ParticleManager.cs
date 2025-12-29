using UnityEngine;
public enum Particle
{
    grass
}
public class ParticleManager : SingletonPattern<ParticleManager>
{
    [SerializeField] ParticleSystem _grassPartical;
    public void PlayParticle(Particle type, Vector3 pos)
    {
        var ps = Instantiate(_grassPartical);
        ps.transform.position = pos;
        ps.Play();
    }
}
