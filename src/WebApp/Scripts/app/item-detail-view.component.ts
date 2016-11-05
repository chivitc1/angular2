import {Component, OnInit} from "@angular/core";
import {Router, ActivatedRoute} from "@angular/router";
import {Item} from "./item";
import {ItemService} from "./item.service";

@Component({
    selector: "item-detail-view",
    template: `
        <div *ngIf="item" class="item-container">
            <div class="item-tab-menu">
                <span (click)="onItemDetailEdit(item)">Edit</span>
                <span class="selected">View</span>
            </div>
            <div class="item-details">
                <div class="mode">Display mode</div>
                <h2>{{item.Title}}</h2>
                <p>{{item.Description}}</p>
            </div>
        </div>
    `,
    styles: [`
        .item-container{ width: 600px }
        .item-tab-menu { margin-right: 30px}
        .item-tab-menu span {
             background-color: #dddddd;
            border: 1px solid #666666;
            border-bottom: 0;
            cursor: pointer;
            display: block;
            float: right;
            margin: 0 0 -1px 5px;
            padding: 5px 10px 4px 10px;
            text-align: center;
            width: 60px;
        }
        .item-tab-menu span.selected {
             background-color: #eeeeee;
            cursor: auto;
            font-weight: bold;
            padding-bottom: 5px;
        }
        .item-details {
            background-color: #eeeeee;
            border: 1px solid black;
            clear: both;
            margin: 0;
            padding: 5px 10px;
        }
        .item-details * {
            vertical-align: middle;
        }
        .item-details .mode {
            font-size: 0.8em;
            color: #777777;
        }
        .item-details ul li {
            padding: 5px 0;
        }
    `]
})

export class ItemDetailViewComponent {

    // decorator input to make item ready for property binding
    // "item" is optional alias parameter
    // alias parameter will be used to determine the attribute hosting the property binding as below
    // <item-detail *ngIf="selectedItem" [item]="selectedItem"></item-detail>
    // it can be skipped as long as the attribute name matches the
    // target input property name.In our case, they share the same name, so we
    // could safely remove it.

    item: Item; 
    constructor(
        private itemService: ItemService,
        private router: Router,
        private activatedRoute: ActivatedRoute
    ) { }

    //hook on Item-Detail component created
    ngOnInit() {
        var id = +this.activatedRoute.snapshot.params["id"];

        //Display detail view
        if (id) {
            this.itemService.get(id)
                .subscribe(
                    item => this.item = item
                );
        }
        //Switch to Edit mode screen
        else if (id === 0) {
            console.log("id is 0: switching to edit mode...");
            this.router.navigate(["item/edit", 0]);
        }
        //Bad route to detail
        else {
            console.log("Invalid id: routing back to home...");
            this.router.navigate([""]);
        }
    }
    
    //Switch to Edit screen
    onItemDetailEdit(item: Item) {
        this.router.navigate(["item/edit", item.Id]);
    }

}
