using UnityEngine;
public enum Particle
{
    grass,
    crop
}
public class ParticleManager : SingletonPattern<ParticleManager>
{
    [SerializeField] ParticleSystem _grassPartical;
    [SerializeField] ParticleSystem _cropPartical;
    public void PlayOneShotParticle(Particle type, Vector3 pos)
    {
        ParticleSystem ps = null;
        switch (type)
        {
            case Particle.grass:
                ps = Instantiate(_grassPartical);
                break;
        }
        if (ps != null) { 
            ps.transform.position = pos;
            ps.Play();
        }
        
    }
    public void PlayCropPartical(Sprite sprite, Vector3 pos) { 
        var ps = Instantiate(_cropPartical);
        ps.transform.position = pos;
        var tsa = ps.textureSheetAnimation;

        tsa.mode = ParticleSystemAnimationMode.Sprites;
        tsa.RemoveSprite(0);
        tsa.AddSprite(sprite);
        ps.Play();
    }
}
