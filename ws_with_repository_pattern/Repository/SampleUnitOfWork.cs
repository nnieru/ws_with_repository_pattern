using ws_with_repository_pattern.DbContext;

namespace ws_with_repository_pattern.Repository;

public class SampleUnitOfWork: IDisposable
{
    private SampleDbContext context = new SampleDbContext();
    private SampleRepository _sampleRepository;
    private bool disposed = false;


    public SampleRepository SampleRepositoryImp
    {
        get
        {
            if (this._sampleRepository == null)
            {
                this._sampleRepository = new SampleRepository(context);
            }

            return _sampleRepository;
        }
    }

    public void Save()
    {
        context.SaveChanges();
    }

    public virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }

        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}