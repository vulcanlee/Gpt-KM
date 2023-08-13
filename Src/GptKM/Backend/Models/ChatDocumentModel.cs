using Backend.Attributes.Validations;
using GptLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class ChatDocumentModel
    {
        public List<ChatDocumentItem> ChatDocumentItem { get; set; } = new List<ChatDocumentItem>();

        public ChatDocumentModel AddUserContent(string content)
        {
            ChatDocumentItem.Add(new ChatDocumentItem
            {
                ChatDocumentSource = ChatDocumentSource.User,
                Content = content
            }.GenerateSourceModifyClass());
            return this;
        }   

        public ChatDocumentModel AddGPTContent(string content)
        {
            ChatDocumentItem.Add(new ChatDocumentItem
            {
                ChatDocumentSource = ChatDocumentSource.GPT,
                Content = content
            }.GenerateSourceModifyClass());
            return this;
        }
    }

    public class ChatDocumentItem
    {
        public ChatDocumentSource ChatDocumentSource { get; set; } 
        public string SourceModifyClass { get; set; }=string.Empty;
        public string Content { get; set; } = string.Empty;

        public ChatDocumentItem GenerateSourceModifyClass()
        {
            SourceModifyClass = ChatDocumentSource switch
            {
                ChatDocumentSource.User => "alert-secondary ms-5",
                ChatDocumentSource.GPT => "alert-primary me-5",
                _ => throw new NotImplementedException(),
            };
            return this;
        }
    }

    public enum ChatDocumentSource
    {
        User,
        GPT
    }
}
