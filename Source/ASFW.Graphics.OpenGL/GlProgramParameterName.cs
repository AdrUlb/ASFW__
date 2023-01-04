using GLenum = System.UInt32;
using static ASFW.Graphics.OpenGL.Constants;

namespace ASFW.Graphics.OpenGL;

public enum GlProgramParameterName : GLenum
{
	DeleteStatus = GL_DELETE_STATUS,
	LinkStatus = GL_LINK_STATUS,
	ValidateStatus = GL_VALIDATE_STATUS,
	InfoLogLength = GL_INFO_LOG_LENGTH,
	AttachedShaders = GL_ATTACHED_SHADERS,
	ActiveAttributes = GL_ACTIVE_ATTRIBUTES,
	ActiveAttributeMaxLength = GL_ACTIVE_ATTRIBUTE_MAX_LENGTH,
	ActiveUniforms = GL_ACTIVE_UNIFORMS,
	ActiveUniformBlocks = GL_ACTIVE_UNIFORM_BLOCKS,
	ActiveUniformBlockMaxNameLength = GL_ACTIVE_UNIFORM_BLOCK_MAX_NAME_LENGTH,
	ActiveUniformMaxLength = GL_ACTIVE_UNIFORM_MAX_LENGTH,
	TransformFeedbackBufferMode = GL_TRANSFORM_FEEDBACK_BUFFER_MODE,
	TransformFeedbackVaryings = GL_TRANSFORM_FEEDBACK_VARYINGS,
	TransformFeedbackVaryingMaxLength = GL_TRANSFORM_FEEDBACK_VARYING_MAX_LENGTH,
	GeometryVerticesOut = GL_GEOMETRY_VERTICES_OUT,
	GeometryInputType = GL_GEOMETRY_INPUT_TYPE,
	GeometryOutputType = GL_GEOMETRY_OUTPUT_TYPE
}
