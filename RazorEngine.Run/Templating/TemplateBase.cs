﻿namespace RazorEngine.Templating
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Provides a base implementation of a template.
    /// </summary>
    public abstract class TemplateBase : MarshalByRefObject, ITemplate
    {
        #region Fields
        private ExecuteContext _context;
        #endregion

        #region Constructor
        /// <summary>
        /// Initialises a new instance of <see cref="TemplateBase"/>.
        /// </summary>
        protected TemplateBase() { }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the layout template name.
        /// </summary>
        public string _Layout { get; set; }

        /// <summary>
        /// Gets or sets the template service.
        /// </summary>
        public ITemplateService TemplateService { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Defines a section that can written out to a layout.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <param name="action">The delegate used to write the section.</param>
        public void DefineSection(string name, Action action)
        {
            _context.DefineSection(name, action);
        }

        /// <summary>
        /// Includes the template with the specified name.
        /// </summary>
        /// <param name="name">The name of the template to include.</param>
        /// <returns>The template writer helper.</returns>
        public virtual TemplateWriter Include(string name)
        {
            var instance = TemplateService.Resolve(name);
            if (instance == null)
                throw new ArgumentException("No template could be resolved with name '" + name + "'");

            return new TemplateWriter(tw => tw.Write(instance.Run(new ExecuteContext())));
        }

        /// <summary>
        /// Includes the template with the specified name.
        /// </summary>
        /// <param name="name">The name of the template to include.</param>
        /// <param name="model">The model to pass to the template.</param>
        /// <returns>The template writer helper.</returns>
        public TemplateWriter Include(string name, object model)
        {
            var instance = TemplateService.Resolve(name, model);
            if (instance == null)
                throw new ArgumentException("No template could be resolved with name '" + name + "'");

            return new TemplateWriter(tw => tw.Write(instance.Run(new ExecuteContext())));
        }

        /// <summary>
        /// Includes the template with the specified name.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="name">The name of the template to include.</param>
        /// <param name="model">The model to pass to the template.</param>
        /// <returns>The template writer helper.</returns>
        public TemplateWriter Include<T>(string name, T model)
        {
            var instance = TemplateService.Resolve(name, model);
            if (instance == null)
                throw new ArgumentException("No template could be resolved with name '" + name + "'");

            return new TemplateWriter(tw => tw.Write(instance.Run(new ExecuteContext())));
        }

        /// <summary>
        /// Executes the compiled template.
        /// </summary>
        public virtual void Execute() { }

        public string RunWithNewContext(out object ctx)
        {
            ExecuteContext context = new ExecuteContext();            
            string result = Run(context);
            ctx = context;
            return result;
        }

        public string RunWithExistingContext(object existingCtx, out object ctx)
        {
            ExecuteContext context = (ExecuteContext)existingCtx;
            string result = Run(context);
            ctx = context;
            return result;
        }

        /// <summary>
        /// Runs the template and returns the result.
        /// </summary>
        /// <param name="context">The current execution context.</param>
        /// <returns>The merged result of the template.</returns>
        public string Run(ExecuteContext context)
        {
            _context = context;

            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {
                //_context.PushWriter(writer);
                _context.CurrentWriter = writer;
                Execute();
                //_context.PopWriter();
                _context.CurrentWriter = null;
            }

            if (_Layout != null)
            {
                //var layout = 
                //var body = new TemplateWriter(tw => tw.Write(builder.ToString()));
                //context.PushBody(body);

                //return layout.Run(context);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Renders the section with the specified name.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <param name="isRequired">Flag to specify whether the section is required.</param>
        /// <returns>The template writer helper.</returns>
        public TemplateWriter RenderSection(string name, bool isRequired = true)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The name of the section to render must be specified.");

            var action = _context.GetSectionDelegate(name);
            if (action == null && isRequired)
                throw new ArgumentException("No section has been defined with name '" + name + "'");

            if (action == null) action = () => { };

            return new TemplateWriter(tw => action());
        }

        /// <summary>
        /// Renders the body of the template.
        /// </summary>
        /// <returns>The template writer helper.</returns>
        public TemplateWriter RenderBody()
        {
            return _context.PopBody();
        }

        /// <summary>
        /// Writes the specified object to the result.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public virtual void Write(object value)
        {
            if (value == null) return;

            var encodedString = value as string;
            if (encodedString != null)
            {
                _context.CurrentWriter.Write(encodedString);
            }
            else
            {
            //    encodedString = TemplateService.EncodedStringFactory.CreateEncodedString(value);
            //    _context.CurrentWriter.Write(encodedString);
            }
        }

        /// <summary>
        /// Writes the specified template helper result.
        /// </summary>
        /// <param name="helper">The template writer helper.</param>
        public virtual void Write(TemplateWriter helper)
        {
            helper.WriteTo(_context.CurrentWriter);
        }

        /// <summary>
        /// Writes the specified string to the result.
        /// </summary>
        /// <param name="literal">The literal to write.</param>
        public virtual void WriteLiteral(string literal)
        {
            if (literal == null) return;
            _context.CurrentWriter.Write(literal);
        }

        /// <summary>
        /// Writes a string literal to the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="literal">The literal to be written.</param>
        public static void WriteLiteralTo(TextWriter writer, string literal)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (literal == null) return;
            writer.Write(literal);
        }

        /// <summary>
        /// Writes the specified object to the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteTo(TextWriter writer, object value)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (value == null) return;
            writer.Write(value);
        }

        /// <summary>
        /// Writes the specfied template helper result to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="helper">The template writer helper.</param>
        public static void WriteTo(TextWriter writer, TemplateWriter helper)
        {
            helper.WriteTo(writer);
        }
        #endregion

        //ITemplate resolve(string name)
        //{
        //    ITemplate instance = CreateTemplate();

        //    //ITemplate instance = null;
        //    //if (_cache.TryGetValue(name, out cachedItem))
        //    //    instance = CreateTemplate(cachedItem.TemplateType);

        //    //if (instance == null && _config.Resolver != null)
        //    //{
        //    //    string template = _config.Resolver.Resolve(name);
        //    //    if (!string.IsNullOrWhiteSpace(template))
        //    //        instance = GetTemplate(template, name);
        //    //}

        //    return instance;
        //}

        //protected virtual ITemplate CreateTemplate(Type templateType)
        //{
        //   // var context = CreateInstanceContext(templateType);
        //    var instance = context..Activator.CreateInstance(context);
        //    instance.TemplateService = this;

        //    return instance;
        //}


        //protected virtual InstanceContext CreateInstanceContext(Type templateType)
        //{
        //    return new InstanceContext(templateType);
        //}
    }
}