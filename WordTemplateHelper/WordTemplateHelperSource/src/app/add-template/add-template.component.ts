

import { Component, } from '@angular/core';
import { Router } from '@angular/router';

import { FabricTextFieldWrapperComponent } from '../shared/office-fabric-component-wrappers/fabric.textfield.wrapper.component';
import { ButtonComponent } from '../shared/button/button.component';
import { NavigationHeaderComponent } from '../shared/navigation-header/navigation.header.component';
import { BrandFooterComponent } from '../shared/brand-footer/brand.footer.component';

// The WordDocumentService provides methods for manipulating the document.
import { WordDocumentService } from '../services/word-document/word.document.service';
// The SettingsStorageService provides CRUD operations on application settings..
import { SettingsStorageService, WordTemplateApiService } from '../services/index';
import { BaseResponseResultInfo, WordTemplateInfo, TemplateType } from '../models/index';

@Component({
    templateUrl: './add-template.component.html',
    styleUrls: ['./add-template.component.css']
})
export class AddTemplateComponent {

    public templateContent: string;
    public templateType: TemplateType;
    public templateName: string;
    public message: string;
    public isShowMessage: boolean;

    constructor(private wordDocument: WordDocumentService,
        private settingsStorage: SettingsStorageService,
        public wordTemplateApiService: WordTemplateApiService,
        private router: Router) {
        this.templateName = "";
        this.isShowMessage = false;
    }

    // onTemplateNameEntered(message: string): void {
    //     this.templateName = message;
    // }

    addTemplate() {
        if (this.templateName == "") {
            this.message = "Please input the template name!";
            this.isShowMessage = true;
        }
        else if (this.templateType == null || this.templateType == undefined) {
            this.message = "Please select the template type!";
            this.isShowMessage = true;
        }
        else {
            let tpl: WordTemplateInfo = new WordTemplateInfo();
            

            //TODO
            this.wordDocument.getOoxml().then(value => {
                tpl.downloadCount = 0;
                tpl.imageUrl = "";
                tpl.name = this.templateName;
                tpl.star = 0;
                tpl.templateContent = value;
                tpl.type = this.templateType;
                this.wordTemplateApiService.addWordTemplate(this.templateType, tpl)
                    .then(response => {
                        if (response.isSuccess) {
                            this.message = "Upload done!";
                            this.isShowMessage = true;
                            this.templateName = "";
                        }
                        else {
                            this.message = "Upload failed!" + response.message;
                            this.isShowMessage = true;
                        }
                    })
                    .catch(error => {
                        this.message = "Upload failed!";
                        this.isShowMessage = true;
                    })
            })

        }
    }

    closeMessage() {
        this.message = "";
        this.isShowMessage = false;
    }

    setPrivateType() {
        this.templateType = TemplateType.Private;
    }

    setPublicType() {
        this.templateType = TemplateType.Public;
    }

    setOrganizationType() {
        this.templateType = TemplateType.Organization;
    }
}
